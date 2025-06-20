using NewsAggrigationClient.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigationClient.Views
{
    public class AdminMenu
    {
        private readonly IAdminOperation _adminOperation;

        public AdminMenu(IAdminOperation adminOperation)
        {
            _adminOperation = adminOperation;
        }
        public async Task ShowAdminMenuAsync()
        {
            while (true)
            {
                Console.WriteLine("\nAdmin Menu");
                Console.WriteLine("1. View External Servers");
                Console.WriteLine("2. View Server Details");
                Console.WriteLine("3. Update/Edit Server");
                Console.WriteLine("4. Add News Category");
                Console.WriteLine("5. Logout");

                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        await _adminOperation.ViewAllExternalServersAsync();
                        break;
                    case "2":
                        await _adminOperation.ViewExternalServerDetailsAsync();
                        break;
                    case "3":
                        await _adminOperation.UpdateExternalServerAsync();
                        break;
                    case "4":
                        await _adminOperation.AddCategoryAsync();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
    }
}
