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
                ShowMenuOptions();

                Console.Write("Choose an option: ");
                var choice = Console.ReadLine();

                if (await ExecuteMenuOptionAsync(choice) == false)
                    return;
            }
        }

        private void ShowMenuOptions()
        {
            Console.Clear();
            Console.WriteLine("  Admin Menu");
            Console.WriteLine("1. View External Servers");
            Console.WriteLine("2. View Server Details");
            Console.WriteLine("3. Update/Edit Server");
            Console.WriteLine("4. Add News Category");
            Console.WriteLine("5. View Reported Articles");
            Console.WriteLine("6. Hide/Unhide Articles");
            Console.WriteLine("7. Hide/Unhide Categories");
            Console.WriteLine("8. Hide/Unhide Keywords");
            Console.WriteLine("9. Block Articles by Keyword");
            Console.WriteLine("10. Logout");
        }

        private async Task<bool> ExecuteMenuOptionAsync(string? input)
        {
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
                    await _adminOperation.ViewReportedArticlesAsync();
                    break;
                case "6":
                    await _adminOperation.ToggleArticleVisibilityAsync();
                    break;
                case "7":
                    await _adminOperation.ToggleCategoryVisibilityAsync();
                    break;
                case "8":
                    await _adminOperation.ToggleKeywordVisibilityAsync();
                    break;
                case "9":
                    await _adminOperation.BlockArticlesByKeywordAsync();
                    break;
                case "10":
                    Console.WriteLine(" Logged out successfully.");
                    return false;
                default:
                    Console.WriteLine(" Invalid option. Please try again.");
                    break;
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            return true;
        }
    }
}
