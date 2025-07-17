using NewsAggrigation.API.DataDTOs;
using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.API.ServiceDTOs.ResponseDTOs;
using NewsAggrigation.BLL.Services.Helper.UserIdentity;
using NewsAggrigation.DAL.Models;
using NewsAggrigation.DAL.Repositories.CategoryRepo;
using NewsAggrigation.DAL.Repositories.NotificationRepo;

namespace NewsAggrigation.BLL.Services.Notification
{
    public class NotificationService : INotificationService
    {
        public INotificationRepository _notificationRepo;
        public ICategoryRepository _categoryRepo;
        private readonly IUserIdentityContext _userIdentityContext;

        public NotificationService(INotificationRepository notificationRepo, IUserIdentityContext userIdentityContext, ICategoryRepository categoryRepo)
        {
            _notificationRepo = notificationRepo;
            _userIdentityContext = userIdentityContext;
            _categoryRepo = categoryRepo;
        }

        public async Task<List<NotificationResponse>> GetUserNotificationsAsync(int userId)
        {
            var notifications = await _notificationRepo.GetUserNotificationsAsync(userId);
            await _notificationRepo.HideNotificationAsync(notifications);

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
            foreach (var pair in request.CategorySettings)
            {
                if(string.Equals(pair.Key, "keywords", StringComparison.OrdinalIgnoreCase))
                {
                    var existingKeywords = await _notificationRepo.GetUserKeywordsAsync(request.UserId);
                    if (existingKeywords == null) continue;

                    foreach (var keyword in existingKeywords)
                    {
                        keyword.IsEnabled = pair.Value;
                    }

                    await _notificationRepo.UpdateUserKeywordSettingsAsync(existingKeywords);
                    continue;
                }
                var category = await _notificationRepo.GetCategoryByNameAsync(pair.Key);
                if (category == null) continue;

                var existingSetting = await _notificationRepo.GetCategorySettingAsync(request.UserId, category.CategoryId);
                if (existingSetting == null)
                {
                    await _notificationRepo.AddCategorySettingAsync(new CategoryNotificationSetting
                    {
                        UserId = request.UserId,
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
            var existingKeywords = await _notificationRepo.GetUserKeywordsAsync(request.UserId);
            var existingDict = existingKeywords
                .ToDictionary(k => k.Word, StringComparer.OrdinalIgnoreCase);

            var inputKeywords = request.Keywords.Distinct(StringComparer.OrdinalIgnoreCase);

            var newKeywordEntities = new List<Keyword>();

            foreach (var word in inputKeywords)
            {
                if (existingDict.TryGetValue(word, out var existingKeyword))
                {
                    // If keyword exists but is disabled 
                    if (!existingKeyword.IsEnabled)
                    {
                        existingKeyword.IsEnabled = true;
                        existingKeyword.IsDeleted = false;
                    }
                }
                else
                {
                    newKeywordEntities.Add(new Keyword
                    {
                        UserId = request.UserId,
                        Word = word,
                        IsEnabled = true,
                        IsDeleted = false
                    });
                }
            }

            if (newKeywordEntities.Any())
                await _notificationRepo.AddKeywordsAsync(newKeywordEntities);

            await _notificationRepo.SaveChangesAsync();
        }

        public async Task<NotificationConfigResponse> GetUserNotificationConfigAsync(int userId)
        {
            var categories = await _categoryRepo.GetAllAsync();
            var categoryConfig = await _notificationRepo.GetUserCategoryNotificationPreferencesAsync(userId);

            var categoryStatus = categories.Select(category => new CategoryStatusDto
            {
                CategoryName = category.CategoryName,
                IsEnabled = categoryConfig.Contains(category.CategoryName)
            }).ToList();

            var keywordConfig = await _notificationRepo.GetUserKeywordNotificationPreferencesAsync(userId);
            var keywordCategory = new CategoryStatusDto
            {
                CategoryName = "Keywords",
                IsEnabled = keywordConfig.Any(k => k.IsEnabled)
            };

            categoryStatus.Add(keywordCategory);
            return new NotificationConfigResponse
            {
                Categories = categoryStatus,
                Keywords = keywordConfig.Select(k => k.Word).ToList()
            };
        }
    }
}
