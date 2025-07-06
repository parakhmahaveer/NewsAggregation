using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.API.ServiceDTOs.ResponseDTOs;
using NewsAggrigation.DAL.Models;
using NewsAggrigation.DAL.Repositories.NotificationRepo;

namespace NewsAggrigation.BLL.Services.Notification
{
    public class NotificationService : INotificationService
    {
        public INotificationRepository _notificationRepo;

        public NotificationService(INotificationRepository notificationRepo)
        {
            _notificationRepo = notificationRepo;
        }

        public async Task<List<NotificationResponse>> GetUserNotificationsAsync(string username)
        {
            var notifications = await _notificationRepo.GetUserNotificationsAsync(username);

            return notifications.Select(n => new NotificationResponse
            {
                NotificationId = n.NotificationId,
                Title = n.Article.Title,
                SentDate = n.SentDate,
                Source = n.Article.Source,
                Url = n.Article.Url
            }).ToList();
        }

        public async Task ConfigureCategoryNotificationAsync(ConfigureCategoryNotificationRequest request)
        {
            var user = await _notificationRepo.GetUserByUsernameAsync(request.Username)
                       ?? throw new ArgumentException("User not found");

            foreach (var pair in request.CategorySettings)
            {
                var category = await _notificationRepo.GetCategoryByNameAsync(pair.Key);
                if (category == null) continue;

                var existingSetting = await _notificationRepo.GetCategorySettingAsync(user.UserId, category.CategoryId);
                if (existingSetting == null)
                {
                    await _notificationRepo.AddCategorySettingAsync(new CategoryNotificationSetting
                    {
                        UserId = user.UserId,
                        CategoryId = category.CategoryId,
                        IsEnabled = pair.Value,
                        IsDeleted = false
                    });
                }
                else
                {
                    existingSetting.IsEnabled = pair.Value;
                }
            }

            await _notificationRepo.SaveChangesAsync();
        }

        public async Task ConfigureKeywordNotificationAsync(ConfigureKeywordNotificationRequest request)
        {
            var user = await _notificationRepo.GetUserByUsernameAsync(request.Username)
                       ?? throw new ArgumentException("User not found");

            var existingKeywords = await _notificationRepo.GetUserKeywordsAsync(user.UserId);
            await _notificationRepo.RemoveKeywordsAsync(existingKeywords);

            var newKeywords = request.Keywords.Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(k => new Keyword
                {
                    UserId = user.UserId,
                    Word = k,
                    IsEnabled = request.IsEnabled,
                    IsDeleted = false
                });

            await _notificationRepo.AddKeywordsAsync(newKeywords);
            await _notificationRepo.SaveChangesAsync();
        }

        public async Task<NotificationConfigResponse> GetUserNotificationConfigAsync(int userId)
        {
            var categories = await _notificationRepo.GetUserCategoryNotificationPreferencesAsync(userId);
            var keywords = await _notificationRepo.GetUserKeywordNotificationPreferencesAsync(userId);

            return new NotificationConfigResponse
            {
                Categories = categories,
                Keywords = keywords
            };
        }
    }
}
