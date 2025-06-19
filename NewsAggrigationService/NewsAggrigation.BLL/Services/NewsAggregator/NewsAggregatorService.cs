using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggrigation.BLL.Services.Categorizer;
using NewsAggrigation.BLL.Services.NewsAggregator.ExternalApiHandler;
using NewsAggrigation.DAL;

namespace NewsAggrigation.BLL.Services.NewsAggregator
{
    public class NewsAggregatorService : INewsAggregatorService
    {
        private readonly NewsAggregatorDbContext _context;
        private readonly ILogger<NewsAggregatorService> _logger;
        private readonly ICategorizerService _categorizerService;
        private readonly ExternalNewsApiFactoryClient _factory;

        public NewsAggregatorService(NewsAggregatorDbContext context, ILogger<NewsAggregatorService> logger, ICategorizerService categorizerService, ExternalNewsApiFactoryClient factory)
        {
            _context = context;
            _categorizerService = categorizerService;
            _logger = logger;
            _factory = factory;
        }

        public async Task FetchAndStoreNewsFromAllSourcesAsync()
        {
            var externalApiSources = await _context.ExternalAPIConfigs
            .Where(s => s.IsEnable)
            .ToListAsync();

            foreach (var externalApi in externalApiSources)
            {
                try
                {
                    var client = _factory.GetClient(externalApi);
                    var articles = await client.FetchArticlesAsync();

                    foreach (var article in articles)
                    {
                        if (!_context.Articles.Any(a => a.Url == article.Url))
                        {
                            var detectedCategoryId = await _categorizerService.DetectCategoryAsync($"{article.Title} {article.Content}");
                            article.CategoryId = (int)(detectedCategoryId == null ? 9 : detectedCategoryId);
                            _context.Articles.Add(article);
                        }
                    }

                    externalApi.LastAccessedDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("News fetched from {source}", externalApi.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to fetch from source {source}", externalApi.Name);
                }
            }
        }
    }
}
