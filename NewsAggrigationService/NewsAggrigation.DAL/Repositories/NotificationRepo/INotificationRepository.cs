using NewsAggrigation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.DAL.Repositories.NotificationRepo
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetUserNotificationsAsync(string username);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<Category?> GetCategoryByNameAsync(string categoryName);
        Task<CategoryNotificationSetting?> GetCategorySettingAsync(int userId, int categoryId);
        Task AddCategorySettingAsync(CategoryNotificationSetting setting);
        Task<List<Keyword>> GetUserKeywordsAsync(int userId);
        Task AddKeywordsAsync(IEnumerable<Keyword> keywords);
        Task RemoveKeywordsAsync(IEnumerable<Keyword> keywords);
        Task SaveChangesAsync();
    }
}
