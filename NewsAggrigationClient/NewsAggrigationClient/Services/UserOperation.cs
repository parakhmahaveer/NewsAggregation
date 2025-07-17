using NewsAggrigationClient.Models.DTOs.RequestDTOs;
using NewsAggrigationClient.Models.DTOs.ResponseDTOs;
using NewsAggrigationClient.Services.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            try
            {
                var response = await _httpClient.GetAsync("api/news/today");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Failed to fetch today's news.");
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
                    Console.WriteLine($"Likes      : {article.LikeCount} | Dislikes: {article.DislikeCount}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching today's news: {ex.Message}");
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

        public async Task<List<GetCategoriesResponse>> GetAllCategoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/categories");
                if (response.IsSuccessStatusCode)
                {
                    var categories = await response.Content.ReadFromJsonAsync<List<GetCategoriesResponse>>();
                    return categories ?? new List<GetCategoriesResponse>();
                }
                else
                {
                    Console.WriteLine("Failed to load categories.");
                    return new List<GetCategoriesResponse>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while fetching categories: {ex.Message}");
                return new List<GetCategoriesResponse>();
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
            try
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
                    Console.WriteLine($"Likes      : {article.LikeCount} | Dislikes: {article.DislikeCount}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while loading headlines: {ex.Message}");
            }
        }

        public async Task ReactToArticleAsync()
        {
            try
            {
                Console.Write("Enter the Article ID to react: ");
                var articleIdInput = Console.ReadLine();

                if (!int.TryParse(articleIdInput, out int articleId))
                {
                    Console.WriteLine("Invalid article ID.");
                    return;
                }

                Console.Write("Do you want to like or dislike? (like/dislike): ");
                var input = Console.ReadLine()?.Trim().ToLower();

                bool isLiked;
                if (input == "like")
                {
                    isLiked = true;
                }
                else if (input == "dislike")
                {
                    isLiked = false;
                }
                else
                {
                    Console.WriteLine("Invalid input. Type 'like' or 'dislike'.");
                    return;
                }

                var request = new ArticleReactionRequest
                {
                    IsLiked = isLiked,
                    ArticleId = articleId
                };

                var response = await _httpClient.PostAsJsonAsync($"api/news/react", request);

                if (response.IsSuccessStatusCode)
                {
                    var message = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"{message}");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to react. {response.StatusCode}: {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reacting to article: {ex.Message}");
            }
        }

        public async Task SearchArticlesAsync()
        {
            try
            {
                Console.Write("Enter search query: ");
                var query = Console.ReadLine();

                Console.Write("Enter start date (yyyy-MM-dd): ");
                var startDate = Console.ReadLine();

                Console.Write("Enter end date (yyyy-MM-dd): ");
                var endDate = Console.ReadLine();

                Console.Write("Sort by (likes/dislikes): ");
                var sort = Console.ReadLine()?.ToLower();

                // Validate input
                if (!DateTime.TryParse(startDate, out _) || !DateTime.TryParse(endDate, out _))
                {
                    Console.WriteLine("Invalid date format.");
                    return;
                }
                if (!IsStartDateBeforeEndDate(startDate, endDate))
                {
                    Console.WriteLine("Start date must be earlier than or equal to end date.");
                    return;
                }

                var request = new SearchRequest
                {
                    Query = query,
                    StartDate = startDate,
                    EndDate = endDate,
                    SortBy = sort
                };

                var response = await _httpClient.PostAsJsonAsync("api/news/search", request);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Search failed: {response.StatusCode}");
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var articles = JsonSerializer.Deserialize<List<NewsResponse>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (articles == null || articles.Count == 0)
                {
                    Console.WriteLine("No articles found.");
                    return;
                }

                Console.WriteLine("\n Search Results:");
                foreach (var article in articles)
                {
                    Console.WriteLine($"\nID         : {article.ArticleId}");
                    Console.WriteLine($"Title      : {article.Title}");
                    Console.WriteLine($"Source     : {article.Source}");
                    Console.WriteLine($"URL        : {article.Url}");
                    Console.WriteLine($"Category   : {article.Category}");
                    Console.WriteLine($"Likes      : {article.LikeCount} | Dislikes: {article.DislikeCount}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching articles: {ex.Message}");
            }
        }

        public async Task ViewNotificationsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/notifications");
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error: {content}");
                    return;
                }

                var notifications = JsonSerializer.Deserialize<List<NotificationResponse>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (notifications == null || notifications.Count == 0)
                {
                    Console.WriteLine("No notifications found.");
                    return;
                }

                Console.WriteLine("\nYour Notifications:");
                int index = 1;
                foreach (var note in notifications)
                {
                    Console.WriteLine($"\n{index++}. {note.Title}");
                    Console.WriteLine($"    Date   : {note.SentDate:dd MMM yyyy hh:mm tt}");
                    Console.WriteLine($"    Source : {note.Source}");
                    Console.WriteLine($"    URL    : {note.Url}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching notifications: {ex.Message}");
            }
        }

        public async Task SetCategoryNotificationAsync(string category, bool enabled)
        {
            try
            {
                var payload = new
                {
                    CategorySettings = new Dictionary<string, bool>
                    {
                        { category, enabled }
                    }
                };

                var response = await _httpClient.PostAsJsonAsync("api/notifications/configure/category", payload);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Category '{category}' notification {(enabled ? "enabled" : "disabled")}.");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to update category notification: {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating notification category : {ex.Message}");
            }
        }

        public async Task SetKeywordNotificationsAsync(List<string> keywords)
        {
            try
            {
                var payload = new
                {
                    Keywords = keywords
                };

                var response = await _httpClient.PostAsJsonAsync("api/notifications/configure/keyword", payload);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Keyword notifications updated.");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to update keyword notifications: {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating notifications keyword : {ex.Message}");
            }
        }

        public async Task ReportArticleAsync()
        {
            try
            {
                Console.Write("Enter Article ID to report: ");
                if (!int.TryParse(Console.ReadLine(), out int articleId))
                {
                    Console.WriteLine("Invalid Article ID.");
                    return;
                }

                var payload = new { }; // server uses token to resolve user
                var response = await _httpClient.PostAsJsonAsync($"api/news/{articleId}/report", payload);

                Console.WriteLine(response.IsSuccessStatusCode
                    ? "Article reported."
                    : "" + await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reporting article: {ex.Message}");
            }
        }

        public async Task ViewRecommendedArticlesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/news/personalized");
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Unable to fetch personalized articles.");
                    return;
                }

                var articles = await response.Content.ReadFromJsonAsync<List<NewsResponse>>();
                foreach (var article in articles)
                {
                    Console.WriteLine($"\nID         : {article.ArticleId}");
                    Console.WriteLine($"Title      : {article.Title}");
                    Console.WriteLine($"Source     : {article.Source}");
                    Console.WriteLine($"URL        : {article.Url}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching recommended articles: {ex.Message}");
            }
        }

        public async Task<NotificationConfigResponse> ViewNotificationConfigAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/notifications/config");
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Failed to fetch config.");
                    return new NotificationConfigResponse();
                }
                var notificationConfig = await response.Content.ReadFromJsonAsync<NotificationConfigResponse>() ?? new NotificationConfigResponse();
                // Display categories with their status
                Console.WriteLine("\n--- Notification Configuration ---");
                if (notificationConfig.Categories != null && notificationConfig.Categories.Count > 0)
                {
                    int categoryIndex = 1;
                    foreach (var category in notificationConfig.Categories)
                    {
                        Console.WriteLine($"{categoryIndex++}. {category.CategoryName} - {(category.IsEnabled ? "Enabled" : "Disabled")}");
                    }
                }
                else
                {
                    Console.WriteLine("No categories found.");
                }

                // Display keywords
                Console.WriteLine($" Keywords");
                if (notificationConfig.Keywords != null && notificationConfig.Keywords.Count > 0)
                {
                    int index = 1;
                    foreach (var keyword in notificationConfig.Keywords)
                    {
                        Console.WriteLine($"{index++}. {keyword}");
                    }
                }
                else
                {
                    Console.WriteLine("No keywords configured.");
                }

                return notificationConfig;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching notification config: {ex.Message}");
                return new NotificationConfigResponse();
            }
        }

        private bool IsStartDateBeforeEndDate(string startDate, string endDate)
        {
            if (DateTime.TryParse(startDate, out DateTime start) && DateTime.TryParse(endDate, out DateTime end))
            {
                return start <= end;
            }
            return false;
        }
    }
}
