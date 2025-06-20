using NewsAggrigationClient.Configuration;
using NewsAggrigationClient.Services;
using NewsAggrigationClient.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigationClient.Views
{
    public class MainMenu
    {
        private readonly IAuthService _authService;

        public MainMenu(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task ShowMainMenuAsync()
        {
            while (true)
            {
                Console.WriteLine("\n News Aggregator ");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");
                Console.Write("Choose an option: ");

                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        await _authService.RegisterAsync();
                        break;
                    case "2":
                        var token = await _authService.LoginAsync();
                        if (token != null)
                        {
                            var httpClient = HttpClientFactory.CreateClient(token.Token);
                            if (token.Role == "Admin")
                            {
                                IAdminOperation adminOperation = new AdminOperation(httpClient);
                                var adminMenu = new AdminMenu(adminOperation);
                                await adminMenu.ShowAdminMenuAsync();
                            }
                            else
                            {
                                IUserOperation userOperation = new UserOperation(httpClient, token.Username);
                                var userMenu = new UserMenu(userOperation);
                                await userMenu.ShowUserMenuAsync();
                            }
                        }
                        break;
                    case "3":
                        Console.WriteLine("Goodbye");
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }
    }
}
