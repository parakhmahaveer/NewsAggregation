using NewsAggrigationClient.Models.DTOs.RequestDTOs;
using NewsAggrigationClient.Models.DTOs.ResponseDTOs;
using NewsAggrigationClient.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
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
    }
}
