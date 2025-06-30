using NewsAggrigationClient.Models.DTOs.RequestDTOs;
using NewsAggrigationClient.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigationClient.Views
{
    public class UserMenu
    {
        private readonly IUserOperation _userOperation;

        public UserMenu(IUserOperation userOperation)
        {
            _userOperation = userOperation;
        }

        public async Task ShowUserMenuAsync()
        {
            while (true)
            {
                Console.WriteLine($"\nWelcome to the News Application! Date: {DateTime.Now:dd-MMM-yyyy} Time: {DateTime.Now:hh:mm tt}");
                Console.WriteLine("Please choose the options below:");
                Console.WriteLine("1. Headlines");
                Console.WriteLine("2. Saved Articles");
                Console.WriteLine("3. Search");
                Console.WriteLine("4. Notifications");
                Console.WriteLine("5. Logout");
                Console.Write("Choose an option: ");

                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        await ShowHeadlinesMenuAsync();
                        break;

                    case "2":
                        await ShowSavedArticlesMenuAsync();
                        break;

                    case "3":
                        await _userOperation.SearchArticlesAsync();
                        break;

                    case "4":
                        //await _userOperation.ConfigureNotificationsAsync();
                        break;

                    case "5":
                        Console.WriteLine("Logging out...");
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private async Task ShowHeadlinesMenuAsync()
        {
            while (true)
            {
                Console.WriteLine("\nHeadlines");
                Console.WriteLine("1. Today");
                Console.WriteLine("2. Date Range");
                Console.WriteLine("3. Back");
                Console.Write("Choose an option: ");

                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        await _userOperation.ViewTodaysNewsAsync();
                        bool flowControl = await ShowSaveArticleMenuAsync();
                        if (!flowControl)
                        {
                            return;
                        }
                        break;

                    case "2":
                        Console.Write("Enter Start Date (yyyy-MM-dd): ");
                        var startDate = Console.ReadLine();

                        Console.Write("Enter End Date (yyyy-MM-dd): ");
                        var endDate = Console.ReadLine();

                        if (!DateTime.TryParse(startDate, out _) || !DateTime.TryParse(endDate, out _))
                        {
                            Console.WriteLine("Invalid date format.");
                            break;
                        }
                        await ShowHeadlinesByCategoryMenuAsync(startDate, endDate);
                        break;

                    case "3":
                        return;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private async Task ShowHeadlinesByCategoryMenuAsync(string startDate, string endDate)
        {
            var request = new NewsByCategoryRequest
            {
                StartDate = startDate,
                EndDate = endDate
            };
            while (true)
            {
                Console.WriteLine("\nChoose category:");
                Console.WriteLine("1. All");
                Console.WriteLine("2. Business");
                Console.WriteLine("3. Entertainment");
                Console.WriteLine("4. Sports");
                Console.WriteLine("5. Technology");
                Console.WriteLine("6. Back");

                Console.Write("Enter your choice: ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        request.Category = "all";
                        break;
                    case "2":
                        request.Category = "business";
                        break;
                    case "3":
                        request.Category = "entertainment";
                        break;
                    case "4":
                        request.Category = "sports";
                        break;
                    case "5":
                        request.Category = "technology";
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
                await _userOperation.ViewHeadlinesAsync(request);
                bool flowControl = await ShowSaveArticleMenuAsync();
                if (!flowControl)
                {
                    return;
                }
            }
        }

        private async Task<bool> ShowSaveArticleMenuAsync()
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Save Article");
            Console.WriteLine("2. Like or Dislike");
            Console.WriteLine("3. Back");

            var option = Console.ReadLine();
            if (option == "1")
            {
                await _userOperation.SaveArticleAsync();
            }
            else if (option == "2")
            {
                await _userOperation.ReactToArticleAsync();
            }
            else if(option == "3")
            {
                return false;
            }
                return true;
        }

        private async Task ShowSavedArticlesMenuAsync()
        {
            while (true)
            {
                await _userOperation.ViewSavedArticlesAsync();

                Console.WriteLine("\nOptions:");
                Console.WriteLine("1. Delete Article");
                Console.WriteLine("2. Back");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        await _userOperation.DeleteSavedArticleAsync();
                        break;
                    case "2":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }
    }
}
