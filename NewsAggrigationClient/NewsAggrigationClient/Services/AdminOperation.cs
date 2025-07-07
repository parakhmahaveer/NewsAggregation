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

                Console.WriteLine("\nList of external servers:");
                foreach (var api in servers)
                {
                    var status = api.IsActive ? "Active" : "Not Active";
                    var lastAccess = api.LastAccessedAt.ToString("dd MMM yyyy") ?? "N/A";
                    Console.WriteLine($"{api.Id}. {api.ApiName} - {status} - last accessed: {lastAccess}");
                }
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

            Console.WriteLine("\nList of external server details:");
            foreach (var api in servers)
            {
                Console.WriteLine($"{api.Id}. {api.ApiName} - {api.ApiKey}");
            }
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
            var id = Console.ReadLine();
            Console.Write("New API Key: ");
            var apiKey = Console.ReadLine();

            var updateRequest = new ExternalApiUpdateRequest
            {
                ApiKey = apiKey
            };

            var response = await _httpClient.PatchAsync($"api/ExternalApis/{id}",
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
                Console.WriteLine("❌ Failed to fetch reported articles.");
                Console.WriteLine(content);
                return;
            }

            var articles = JsonSerializer.Deserialize<List<NewsResponse>>(content);

            Console.WriteLine("\nReported Articles:");
            int index = 1;
            foreach (var article in articles)
            {
                Console.WriteLine($"\n{index++}. {article.Title}");
                Console.WriteLine($"   Source: {article.Source}");
                //Console.WriteLine($"   Date  : {article.:dd MMM yyyy}");
                Console.WriteLine($"   URL   : {article.Url}");
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
                Console.WriteLine("❌ Invalid option.");
                return;
            }

            var response = await _httpClient.PostAsync(endpoint, null);
            Console.WriteLine(response.IsSuccessStatusCode
                ? "Category visibility updated."
                : $"Failed: {await response.Content.ReadAsStringAsync()}");
        }

        public async Task ToggleKeywordVisibilityAsync()
        {
            Console.Write("Enter Category Keyword ID to toggle visibility: ");
            var input = Console.ReadLine();

            if (!int.TryParse(input, out int keywordId))
            {
                Console.WriteLine("❌ Invalid Keyword ID.");
                return;
            }

            Console.Write("Do you want to (h)ide or (u)nhide the keyword? ");
            var option = Console.ReadLine()?.ToLower();

            string endpoint = option == "h"
                ? $"api/categories/keywords/{keywordId}/hide"
                : option == "u"
                    ? $"api/categories/keywords/{keywordId}/unhide"
                    : null;

            if (endpoint == null)
            {
                Console.WriteLine("❌ Invalid option.");
                return;
            }

            var response = await _httpClient.PostAsync(endpoint, null);
            Console.WriteLine(response.IsSuccessStatusCode
                ? "Keyword visibility updated."
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

            var response = await _httpClient.PostAsync($"api/news/block-keyword?keyword={Uri.EscapeDataString(keyword)}", null);

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
    }
}
