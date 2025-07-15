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
                Console.Clear();
                Console.WriteLine($"\nWelcome to the News Application! Date: {DateTime.Now:dd-MMM-yyyy} Time: {DateTime.Now:hh:mm tt}");
                Console.WriteLine("Please choose the options below:");
                Console.WriteLine("1. Headlines");
                Console.WriteLine("2. Saved Articles");
                Console.WriteLine("3. Search");
                Console.WriteLine("4. Notifications");
                Console.WriteLine("5. Recommended Articles");
                Console.WriteLine("6. Logout");
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
                        await ShowNotificationMenuAsync();
                        break;

                    case "5":
                        await _userOperation.ViewRecommendedArticlesAsync();
                        Console.ReadLine();
                        break;

                    case "6":
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
                        bool flowControl = await ShowArticleActionMenuAsync();
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
                bool flowControl = await ShowArticleActionMenuAsync();
                if (!flowControl)
                {
                    return;
                }
            }
        }

        private async Task<bool> ShowArticleActionMenuAsync()
        {
            while (true)
            {
                Console.WriteLine("\nOptions:");
                Console.WriteLine("1. Save Article");
                Console.WriteLine("2. Like or Dislike");
                Console.WriteLine("3. Report Article");
                Console.WriteLine("4. Back");

                Console.Write("Select an option: ");
                var option = Console.ReadLine()?.Trim();

                switch (option)
                {
                    case "1":
                        await _userOperation.SaveArticleAsync();
                        break;
                    case "2":
                        await _userOperation.ReactToArticleAsync();
                        break;
                    case "3":
                        await _userOperation.ReportArticleAsync();
                        break;
                    case "4":
                        return false; // Exit the menu
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        continue;
                }

                return true;
            }
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

        private async Task ShowNotificationMenuAsync()
        {
            while (true)
            {
                Console.WriteLine("\nNotification:");
                Console.WriteLine("1. View Notifications");
                Console.WriteLine("2. Configure Notifications");
                Console.WriteLine("3. Back");
                Console.WriteLine("4. Logout");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        await _userOperation.ViewNotificationsAsync();
                        break;
                    case "2":
                        await ShowNotificationConfigMenuAsync();
                        break;
                    case "3":
                        return;
                    case "4":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private async Task ShowNotificationConfigMenuAsync()
        {
            while (true)
            {
                Console.WriteLine("\n--- Configure Notifications ---");
                Console.WriteLine("1. Enable/Disable Category Notification");
                Console.WriteLine("2. Set Keywords for Notification");
                Console.WriteLine("3. Back");
                Console.Write("Choose an option: ");

                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        Console.Write("Enter Category Name: ");
                        var category = Console.ReadLine();
                        Console.Write("Enable this category? (y/n): ");
                        var enable = Console.ReadLine()?.Trim().ToLower() == "y";
                        await _userOperation.SetCategoryNotificationAsync(category!, enable);
                        break;

                    case "2":
                        Console.Write("Enter comma-separated keywords: ");
                        var keywords = Console.ReadLine();
                        var keywordList = keywords?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                   .Select(k => k.Trim())
                                                   .ToList() ?? new List<string>();
                        await _userOperation.SetKeywordNotificationsAsync(keywordList);
                        break;

                    case "3":
                        return;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
    }
}
