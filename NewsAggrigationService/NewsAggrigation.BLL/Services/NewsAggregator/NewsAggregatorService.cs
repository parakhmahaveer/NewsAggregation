using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggrigation.BLL.Services.Categorizer;
using NewsAggrigation.BLL.Services.NewsAggregator.ExternalApiHandler;
using NewsAggrigation.DAL;
using NewsAggrigation.DAL.Models;

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
                            await _context.SaveChangesAsync();

                            await NotifySubscribedUsersAsync(article);
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

        private async Task NotifySubscribedUsersAsync(Article article)
        {
            var categoryName = await _context.Categories
                .Where(c => c.CategoryId == article.CategoryId)
                .Select(c => c.CategoryName)
                .FirstOrDefaultAsync();

            var lowerContent = $"{article.Title} {article.Content}".ToLower();

            // 📌 1. Users subscribed to category
            var categoryUserIds = await _context.CategoryNotificationSettings
                .Where(c => c.CategoryId == article.CategoryId && c.IsEnabled && !c.IsDeleted)
                .Select(c => c.UserId)
                .ToListAsync();

            // 📌 2. Users subscribed to keywords found in article
            var keywordUserIds = await _context.Keywords
                .Where(k => k.IsEnabled && !k.IsDeleted && lowerContent.Contains(k.Word.ToLower()))
                .Select(k => k.UserId)
                .ToListAsync();

            // 📌 3. Combine & deduplicate
            var allUserIds = categoryUserIds
                .Concat(keywordUserIds)
                .Distinct()
                .ToList();

            // 📌 4. Create notification records
            foreach (var userId in allUserIds)
            {
                _context.Notifications.Add(new DAL.Models.Notification
                {
                    UserId = userId,
                    ArticleId = article.ArticleId,
                    SentDate = DateTime.UtcNow,
                    IsDeleted = false
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}
