using NewsAggrigation.DAL.Models;

namespace NewsAggrigation.DAL.Repositories.NotificationRepo
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetUserNotificationsAsync(int userId);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<Category?> GetCategoryByNameAsync(string categoryName);
        Task<CategoryNotificationSetting?> GetCategorySettingAsync(int userId, int categoryId);
        Task AddCategorySettingAsync(CategoryNotificationSetting setting);
        Task<List<Keyword>> GetUserKeywordsAsync(int userId);
        Task AddKeywordsAsync(IEnumerable<Keyword> keywords);
        Task RemoveKeywordsAsync(IEnumerable<Keyword> keywords);
        Task SaveChangesAsync();
        Task<List<string>> GetUserCategoryNotificationPreferencesAsync(int userId);
        Task<List<string>> GetUserKeywordNotificationPreferencesAsync(int userId);
    }
}
