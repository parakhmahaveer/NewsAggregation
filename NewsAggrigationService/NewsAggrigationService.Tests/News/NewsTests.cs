using Moq;
using Microsoft.Extensions.Configuration;
using NewsAggrigation.BLL.Services.News;
using NewsAggrigation.DAL.Repositories.ArticleRepo;
using NewsAggrigation.BLL.Exceptions;

namespace NewsAggrigationService.Tests.News
{
    public class NewsTests
    {
        private readonly Mock<IArticleRepository> _mockArticleRepository;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly NewsService _newsService;

        public NewsTests()
        {
            _mockArticleRepository = new Mock<IArticleRepository>();
            _mockConfiguration = new Mock<IConfiguration>();
            _newsService = new NewsService(_mockArticleRepository.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task GetNewsByDateRangeAsync_ShouldThrowValidationException_WhenStartDateIsAfterEndDate()
        {
            var start = new DateTime(2025, 7, 16);
            var end = new DateTime(2025, 7, 10);

            await Assert.ThrowsAsync<ValidationException>(() =>
                _newsService.GetNewsByDateRangeAsync(start, end));
        }

        [Fact]
        public async Task GetSavedArticlesAsync_ShouldThrowValidationException_WhenUsernameIsEmpty()
        {
            string username = "";

            await Assert.ThrowsAsync<ValidationException>(() =>
                _newsService.GetSavedArticlesAsync(username));
        }

        [Fact]
        public async Task SaveArticleAsync_ShouldThrowNotFoundException_WhenArticleNotFound()
        {
            string username = "user1";
            int articleId = 109;

            _mockArticleRepository
                .Setup(r => r.SaveArticleAsync(username, articleId))
                .ThrowsAsync(new NotFoundException("Article not found."));

            var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
                _newsService.SaveArticleAsync(username, articleId));

            Assert.Equal("Article not found.", ex.Message);
        }
    }
}
