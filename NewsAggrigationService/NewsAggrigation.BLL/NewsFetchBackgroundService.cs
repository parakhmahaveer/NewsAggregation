using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsAggrigation.BLL.Services.NewsAggregator;

namespace NewsAggrigation.BLL
{
    public class NewsFetchBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NewsFetchBackgroundService> _logger;

        public NewsFetchBackgroundService(IServiceProvider serviceProvider, ILogger<NewsFetchBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Running News Aggregation...");

                using var scope = _serviceProvider.CreateScope();
                var aggregator = scope.ServiceProvider.GetRequiredService<INewsAggregatorService>();
                await aggregator.FetchAndStoreNewsFromAllSourcesAsync();

                _logger.LogInformation("News Aggregation completed.");

                await Task.Delay(TimeSpan.FromHours(3), stoppingToken);
            }
        }
    }
}
