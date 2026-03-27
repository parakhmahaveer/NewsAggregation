using Microsoft.Extensions.DependencyInjection;
using NewsAggrigation.DAL.Models;

namespace NewsAggrigation.BLL.Services.NewsAggregator.ExternalApiHandler
{
    public class ExternalNewsApiFactoryClient
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly HttpClient _httpClient;

        public ExternalNewsApiFactoryClient(IServiceProvider serviceProvider, HttpClient httpClient)
        {
            _serviceProvider = serviceProvider;
            _httpClient = httpClient;
        }

        public IExternalNewsApiClient GetClient(ExternalAPIConfig source)
        {
            var scopedProvider = _serviceProvider.CreateScope().ServiceProvider;

            return source.Name switch
            {
                "NewsApiOrg" => ActivatorUtilities.CreateInstance<NewsApiClient>(scopedProvider, _httpClient, source),
                //"TheNewsApi" => ActivatorUtilities.CreateInstance<TheNewsApiClient>(scopedProvider, _httpClient, source),
                _ => throw new NotSupportedException($"Source '{source.Name}' is not supported.")
            };
        }
    }
}
