using NewsAggrigationClient.Configuration;
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

        private static string ReadPassword()
        {
            var pwd = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) break;
                if (key.Key == ConsoleKey.Backspace && pwd.Length > 0)
                {
                    pwd.Length--;
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    pwd.Append(key.KeyChar);
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return pwd.ToString();
        }

        public async Task<TokenResponse?> LoginAsync()
        {
            Console.WriteLine("Enter Username:");
            var username = Console.ReadLine()?.Trim();

            Console.WriteLine("Enter Password:");
            var password = ReadPassword();

            var user = new UserLogin
            {
                Username = username,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("api/Auth/Login", user);

            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadFromJsonAsync<TokenResponse>();
                SessionContext.JwtToken = token?.Token;
                SessionContext.Username = token?.Username;
                SessionContext.Role = token?.Role;

                HttpClientFactory.UpdateToken(token?.Token);
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

            var user = new UserRegister
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
