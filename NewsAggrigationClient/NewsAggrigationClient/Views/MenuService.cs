using NewsAggrigationClient.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigationClient.Views
{
    public class MenuService
    {
        private readonly IAuthService _authService;

        public MenuService(IAuthService authService)
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
                            // role-based routing
                            Console.WriteLine(token.Role == "Admin" ? "\nAdmin menu coming soon..." : "\nUser menu coming soon...");
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
