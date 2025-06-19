using NewsAggrigation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace NewsAggrigation.BLL.Services.NewsAggregator.ExternalApiHandler
{
    public class NewsApiClient : IExternalNewsApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ExternalAPIConfig _source;

        public NewsApiClient(HttpClient httpClient, ExternalAPIConfig source)
        {
            _httpClient = httpClient;
            _source = source;
        }

        public async Task<List<Article>> FetchArticlesAsync()
        {
            var url = _source.ApiUrl.Replace("<API_KEY>", _source.ApiKey);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "Mozilla/5.0 (compatible; NewsAggregator/1.0)");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var jsonData = JsonDocument.Parse(content);

            var articles = new List<Article>();

            foreach (var item in jsonData.RootElement.GetProperty("articles").EnumerateArray())
            {
                articles.Add(new Article
                {
                    Title = item.GetProperty("title").GetString(),
                    Content = item.GetProperty("content").GetString() ?? item.GetProperty("description").GetString(),
                    Url = item.GetProperty("url").GetString(),
                    Source = item.GetProperty("source").GetProperty("name").GetString(),
                    PublishedDate = item.GetProperty("publishedAt").GetDateTime()
                });
            }

            return articles;
        }
    }
}
