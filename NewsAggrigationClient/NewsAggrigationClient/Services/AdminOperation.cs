using NewsAggrigationClient.Models.DTOs.RequestDTOs;
using NewsAggrigationClient.Models.DTOs.ResponseDTOs;
using NewsAggrigationClient.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NewsAggrigationClient.Services
{
    public class AdminOperation : IAdminOperation
    {
        private readonly HttpClient _httpClient;

        public AdminOperation(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task ViewAllExternalServersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/ExternalApis");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error: {response.ReasonPhrase}");
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var servers = JsonSerializer.Deserialize<List<ExternalApiResponse>>(json, options);

                if (servers == null || !servers.Any())
                {
                    Console.WriteLine("No external servers found.");
                    return;
                }

                PrintExternalServersTable(servers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public async Task ViewExternalServerDetailsAsync()
        {
            var response = await _httpClient.GetAsync("api/ExternalApis");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {response.ReasonPhrase}");
                return;
            }

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var servers = JsonSerializer.Deserialize<List<ExternalApiResponse>>(json, options);

            if (servers == null || !servers.Any())
            {
                Console.WriteLine("No external servers found.");
                return;
            }

            PrintExternalServerDetails(servers);
        }

        public async Task AddCategoryAsync()
        {
            Console.Write("Enter category name: ");
            var categoryName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                Console.WriteLine("Category name cannot be empty.");
                return;
            }
            try
            {
                var response = await _httpClient.PostAsync($"api/categories?category={Uri.EscapeDataString(categoryName)}", null);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Category created successfully.");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to create category. {response.StatusCode}: {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }
        }

        public async Task UpdateExternalServerAsync()
        {
            Console.Write("Enter API ID to update: ");
            var idInput = Console.ReadLine();

            // Prompt for each field
            Console.Write("New API Key (leave blank to keep unchanged): ");
            var apiKey = Console.ReadLine();

            Console.Write("New API Name (leave blank to keep unchanged): ");
            var apiName = Console.ReadLine();

            Console.Write("New API URL (leave blank to keep unchanged): ");
            var apiUrl = Console.ReadLine();

            // Build the update request
            var updateRequest = new ExternalApiUpdateRequest();

            // Set the ID (required)
            if (int.TryParse(idInput, out int id))
                updateRequest.ApiId = id;
            else
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            // Set only provided fields
            if (!string.IsNullOrWhiteSpace(apiKey))
                updateRequest.ApiKey = apiKey;

            if (!string.IsNullOrWhiteSpace(apiName))
                updateRequest.ApiName = apiName;

            if (!string.IsNullOrWhiteSpace(apiUrl))
                updateRequest.ApiUrl = apiUrl;

            var response = await _httpClient.PatchAsync(
                "api/ExternalApis/update",
                new StringContent(
                    JsonSerializer.Serialize(updateRequest),
                    Encoding.UTF8,
                    "application/json"));

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(response.IsSuccessStatusCode
                ? "API updated successfully."
                : $"Error: {content}");
        }

        public async Task ViewReportedArticlesAsync()
        {
            var response = await _httpClient.GetAsync("api/news/reported");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed to fetch reported articles.");
                Console.WriteLine(content);
                return;
            }

            var articles = JsonSerializer.Deserialize<List<NewsResponse>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (articles == null || !articles.Any())
            {
                Console.WriteLine("No articles found.");
                return;
            }

            Console.WriteLine("====== Reported Articles ======");
            foreach (var article in articles)
            {
                Console.WriteLine($"\nArticle Id: {article.ArticleId}");
                Console.WriteLine($"Title      : {article.Title}");
                Console.WriteLine($"Source     : {article.Source}");
                Console.WriteLine($"URL        : {article.Url}");
                Console.WriteLine($"Category   : {article.Category}");
            }
        }

        public async Task ToggleArticleVisibilityAsync()
        {
            Console.Write("Enter Article ID to toggle visibility: ");
            var input = Console.ReadLine();

            if (!int.TryParse(input, out int articleId))
            {
                Console.WriteLine("Invalid Article ID.");
                return;
            }

            Console.Write("Do you want to (h)ide or (u)nhide the article? ");
            var option = Console.ReadLine()?.ToLower();

            string endpoint = option == "h"
                ? $"api/news/{articleId}/hide"
                : option == "u"
                    ? $"api/news/{articleId}/unhide"
                    : null;

            if (endpoint == null)
            {
                Console.WriteLine("❌ Invalid option.");
                return;
            }

            var response = await _httpClient.PostAsync(endpoint, null);
            Console.WriteLine(response.IsSuccessStatusCode
                ? "Article visibility updated."
                : $"Failed: {await response.Content.ReadAsStringAsync()}");
        }

        public async Task ToggleCategoryVisibilityAsync()
        {
            Console.Write("Enter Category ID to toggle visibility: ");
            var input = Console.ReadLine();

            if (!int.TryParse(input, out int categoryId))
            {
                Console.WriteLine("Invalid Category ID.");
                return;
            }

            Console.Write("Do you want to (h)ide or (u)nhide the category? ");
            var option = Console.ReadLine()?.ToLower();

            string endpoint = option == "h"
                ? $"api/categories/{categoryId}/hide"
                : option == "u"
                    ? $"api/categories/{categoryId}/unhide"
                    : null;

            if (endpoint == null)
            {
                Console.WriteLine("Invalid option.");
                return;
            }

            var response = await _httpClient.PostAsync(endpoint, null);
            Console.WriteLine(response.IsSuccessStatusCode
                ? "Category visibility updated."
                : $"Failed: {await response.Content.ReadAsStringAsync()}");
        }

        public async Task BlockArticlesByKeywordAsync()
        {
            Console.Write("Enter keyword to block articles containing it: ");
            var keyword = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                Console.WriteLine("Keyword cannot be empty.");
                return;
            }

            var response = await _httpClient.PostAsync($"api/categories/block?keyword={Uri.EscapeDataString(keyword)}", null);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Articles containing '{keyword}' were blocked.");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to block keyword: {error}");
            }
        }

        #region Private Methods
        private static void PrintExternalServersTable(List<ExternalApiResponse> servers)
        {
            Console.WriteLine("\n========== External Servers List ==========");
            Console.WriteLine($"Total Servers: {servers.Count}\n");

            // Table column headers
            string header = string.Format("{0,-5} | {1,-25} | {2,-12} | {3,-15}", "Id", "Name", "Status", "Last Accessed");
            Console.WriteLine(header);
            Console.WriteLine(new string('-', header.Length));

            // Table rows
            foreach (var api in servers)
            {
                var status = api.IsActive ? "Active" : "Not Active";
                var lastAccess = api.LastAccessedAt != default
                    ? api.LastAccessedAt.ToString("dd MMM yyyy HH:mm:ss")
                    : "N/A";
                Console.WriteLine(
                    string.Format("{0,-5} | {1,-25} | {2,-12} | {3,-15}",
                        api.Id, api.ApiName, status, lastAccess));
            }

            // Table footer/context
            Console.WriteLine("\n========== End of Server List ==========\n");
        }

        private static void PrintExternalServerDetails(List<ExternalApiResponse> servers)
        {
            Console.WriteLine("\n========== External Server Details ==========");
            Console.WriteLine($"Total Servers: {servers.Count}\n");

            // Table column headers
            string header = string.Format("{0,-5} | {1,-25} | {2,-35}", "Id", "Name", "API Key");
            Console.WriteLine(header);
            Console.WriteLine(new string('-', header.Length));

            // Table rows
            foreach (var api in servers)
            {
                Console.WriteLine(
                    string.Format("{0,-5} | {1,-25} | {2,-35}",
                        api.Id, api.ApiName, api.ApiKey));
            }

            Console.WriteLine("\n========== End of Server Details ==========\n");
        }
        #endregion
    }
}
