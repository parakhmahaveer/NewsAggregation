namespace NewsAggrigation.BLL.Services.NewsAggregator
{
    public interface INewsAggregatorService
    {
        Task FetchAndStoreNewsFromAllSourcesAsync();
    }
}
