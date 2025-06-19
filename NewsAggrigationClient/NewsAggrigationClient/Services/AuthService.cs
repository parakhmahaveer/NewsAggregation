using NewsAggrigationClient.Models.DTOs.RequestDTOs;
using NewsAggrigationClient.Models.DTOs.ResponseDTOs;
using NewsAggrigationClient.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigationClient.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<TokenResponseDto?> LoginAsync()
        {
            Console.WriteLine("Enter Username:");
            var username = Console.ReadLine()?.Trim();

            Console.WriteLine("Enter Password:");
            var password = Console.ReadLine()?.Trim();

            var user = new UserLoginDto
            {
                Username = username,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("api/Auth/Login", user);

            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
                Console.WriteLine($"Login successful. Welcome, {token.Username} ({token.Role})");
                return token;
            }
            else
            {
                Console.WriteLine($"Login failed: {await response.Content.ReadAsStringAsync()}");
                return null;
            }
        }

        public async Task RegisterAsync()
        {
            Console.WriteLine("Enter Username:");
            var username = Console.ReadLine()?.Trim();

            Console.WriteLine("Enter Email:");
            var email = Console.ReadLine()?.Trim();

            Console.WriteLine("Enter Password:");
            var password = Console.ReadLine()?.Trim();

            var user = new UserRegisterDto
            {
                Username = username,
                Email = email,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("api/auth/register", user);

            if (response.IsSuccessStatusCode)
                Console.WriteLine("Registration successful.");
            else
                Console.WriteLine($"Registration failed: {await response.Content.ReadAsStringAsync()}");
        }
    }
}
