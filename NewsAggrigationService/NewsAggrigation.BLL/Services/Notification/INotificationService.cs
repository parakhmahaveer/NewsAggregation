using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.API.ServiceDTOs.ResponseDTOs;

namespace NewsAggrigation.BLL.Services.Notification
{
    public interface INotificationService
    {
        Task<NotificationConfigResponse> GetUserNotificationConfigAsync(int userId);
        Task<List<NotificationResponse>> GetUserNotificationsAsync(string username);
        Task ConfigureCategoryNotificationAsync(ConfigureCategoryNotificationRequest request);
        Task ConfigureKeywordNotificationAsync(ConfigureKeywordNotificationRequest request);
    }
}
