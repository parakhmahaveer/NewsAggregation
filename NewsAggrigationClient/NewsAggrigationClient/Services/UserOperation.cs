using NewsAggrigationClient.Models.DTOs.RequestDTOs;
using NewsAggrigationClient.Models.DTOs.ResponseDTOs;
using NewsAggrigationClient.Services.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace NewsAggrigationClient.Services
{
    public class UserOperation : IUserOperation
    {
        private readonly HttpClient _httpClient;
        private readonly string _username;

        public UserOperation(HttpClient httpClient, string username)
        {
            _httpClient = httpClient;
            _username = username;
        }
        public async Task ViewTodaysNewsAsync()
        {
            var response = await _httpClient.GetAsync("api/news/today");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("❌ Failed to fetch today's news.");
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            var articles = JsonSerializer.Deserialize<List<NewsResponse>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (articles == null || !articles.Any())
            {
                Console.WriteLine("No articles found for today.");
                return;
            }

            Console.WriteLine("\nToday's News:");
            foreach (var article in articles)
            {
                Console.WriteLine($"\nArticle Id: {article.ArticleId}");
                Console.WriteLine($"Title      : {article.Title}");
                Console.WriteLine($"Description: {article.Content}");
                Console.WriteLine($"Source     : {article.Source}");
                Console.WriteLine($"URL        : {article.Url}");
                Console.WriteLine($"Category   : {article.Category}");
            }
        }

        public async Task SaveArticleAsync()
        {
            Console.Write("Enter the Article ID to save: ");
            var articleIdInput = Console.ReadLine();

            if (!int.TryParse(articleIdInput, out int articleId))
            {
                Console.WriteLine("Invalid article ID.");
                return;
            }

            try
            {
                var url = $"api/news/save?username={Uri.EscapeDataString(_username)}&articleId={articleId}";
                var response = await _httpClient.PostAsync(url, null);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Article saved successfully.");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to save article. {response.StatusCode}: {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }
        }

        public async Task DeleteSavedArticleAsync()
        {
            Console.Write("Enter Article ID to unsave: ");
            var articleIdInput = Console.ReadLine();

            if (!int.TryParse(articleIdInput, out int articleId))
            {
                Console.WriteLine("Invalid Article ID.");
                return;
            }

            try
            {
                var url = $"api/news/unsave?username={Uri.EscapeDataString(_username)}&articleId={articleId}";
                var response = await _httpClient.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Article unsaved successfully.");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine("Article not found in saved list.");
                }
                else
                {
                    Console.WriteLine($"Failed to unsave. {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public async Task ViewSavedArticlesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/news/saved/{_username}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Failed to fetch saved articles.");
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();

                var articles = JsonSerializer.Deserialize<List<NewsResponse>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (articles == null || !articles.Any())
                {
                    Console.WriteLine("You have no saved articles.");
                    return;
                }

                Console.WriteLine("\nYour Saved Articles:");
                foreach (var article in articles)
                {
                    Console.WriteLine($"\nArticle Id: {article.ArticleId}");
                    Console.WriteLine($"Title      : {article.Title}");
                    Console.WriteLine($"Description: {article.Content}");
                    Console.WriteLine($"Source     : {article.Source}");
                    Console.WriteLine($"URL        : {article.Url}");
                    Console.WriteLine($"Category   : {article.Category}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public async Task ViewHeadlinesAsync(NewsByCategoryRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/news/category", request);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed to fetch articles.");
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            var articles = JsonSerializer.Deserialize<List<NewsResponse>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (articles == null || !articles.Any())
            {
                Console.WriteLine("No articles found.");
                return;
            }

            Console.WriteLine($"\n{request.Category?.ToUpper() ?? "ALL"} Articles ({request.StartDate} to {request.EndDate}):");
            foreach (var article in articles)
            {
                Console.WriteLine($"\nArticle Id: {article.ArticleId}");
                Console.WriteLine($"Title      : {article.Title}");
                Console.WriteLine($"Description: {article.Content}");
                Console.WriteLine($"Source     : {article.Source}");
                Console.WriteLine($"URL        : {article.Url}");
                Console.WriteLine($"Category   : {article.Category}");
            }
        }
    }
}
