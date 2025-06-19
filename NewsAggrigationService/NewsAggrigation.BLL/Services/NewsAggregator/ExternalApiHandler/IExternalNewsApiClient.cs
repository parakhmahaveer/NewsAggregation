using NewsAggrigation.DAL.Models;

namespace NewsAggrigation.BLL.Services.NewsAggregator.ExternalApiHandler
{
    public interface IExternalNewsApiClient
    {
        Task<List<Article>> FetchArticlesAsync();
    }
}
