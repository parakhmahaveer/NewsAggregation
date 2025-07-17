using FluentAssertions;
using Moq;
using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.BLL.Services.Helper.UserIdentity;
using NewsAggrigation.BLL.Services.Notification;
using NewsAggrigation.DAL.Models;
using NewsAggrigation.DAL.Repositories.CategoryRepo;
using NewsAggrigation.DAL.Repositories.NotificationRepo;

namespace NewsAggrigationService.Tests.NotificationTests
{
    public class NotificationTests
    {
        private readonly Mock<INotificationRepository> _mockNotificationRepo = new();
        private readonly Mock<ICategoryRepository> _mockCategoryRepo = new();
        private readonly Mock<IUserIdentityContext> _mockUserIdentity = new();
        private readonly NotificationService _service;

        public NotificationTests()
        {
            _service = new NotificationService(
                _mockNotificationRepo.Object,
                _mockUserIdentity.Object,
                _mockCategoryRepo.Object
            );
        }

        [Fact]
        public async Task GetUserNotificationsAsync_ShouldReturnMappedNotifications()
        {
            var userId = 1;
            var notifications = new List<Notification>
            {
                new Notification
                {
                    NotificationId = 1,
                    SentDate = DateTime.UtcNow,
                    Article = new Article { Title = "Test", Source = "BBC", Url = "url" }
                }
            };

            _mockNotificationRepo.Setup(r => r.GetUserNotificationsAsync(userId))
                .ReturnsAsync(notifications);

            var result = await _service.GetUserNotificationsAsync(userId);

            result.Should().HaveCount(1);
            result.First().Title.Should().Be("Test");
            _mockNotificationRepo.Verify(r => r.HideNotificationAsync(notifications), Times.Once);
        }

        [Fact]
        public async Task ConfigureCategoryNotificationAsync_ShouldAddOrUpdateSettings()
        {
            var request = new ConfigureCategoryNotificationRequest
            {
                UserId = 1,
                CategorySettings = new Dictionary<string, bool>
                {
                    { "Tech", true },
                    { "Politics", false }
                }
            };

            _mockNotificationRepo.Setup(r => r.GetCategoryByNameAsync("Tech"))
                .ReturnsAsync(new Category { CategoryId = 1, CategoryName = "Tech" });
            _mockNotificationRepo.Setup(r => r.GetCategoryByNameAsync("Politics"))
                .ReturnsAsync(new Category { CategoryId = 2, CategoryName = "Politics" });

            _mockNotificationRepo.Setup(r => r.GetCategorySettingAsync(1, 1))
                .ReturnsAsync((CategoryNotificationSetting?)null);
            _mockNotificationRepo.Setup(r => r.GetCategorySettingAsync(1, 2))
                .ReturnsAsync(new CategoryNotificationSetting { IsEnabled = true });

            await _service.ConfigureCategoryNotificationAsync(request);

            _mockNotificationRepo.Verify(r => r.AddCategorySettingAsync(It.Is<CategoryNotificationSetting>(s => s.CategoryId == 1 && s.IsEnabled)), Times.Once);
            _mockNotificationRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ConfigureKeywordNotificationAsync_ShouldReplaceKeywords()
        {
            var request = new ConfigureKeywordNotificationRequest
            {
                UserId = 1,
                Keywords = new List<string> { "news", "tech", "news" }
            };

            var existing = new List<Keyword> { new Keyword { Word = "old" } };

            _mockNotificationRepo.Setup(r => r.GetUserKeywordsAsync(1)).ReturnsAsync(existing);

            await _service.ConfigureKeywordNotificationAsync(request);

            _mockNotificationRepo.Verify(r => r.RemoveKeywordsAsync(existing), Times.Once);
            _mockNotificationRepo.Verify(r => r.AddKeywordsAsync(It.Is<IEnumerable<Keyword>>(k => k.Count() == 2)), Times.Once);
            _mockNotificationRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
