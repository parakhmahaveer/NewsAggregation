using FluentAssertions;
using Moq;
using NewsAggrigation.BLL.Services.ExternalApi;
using NewsAggrigation.BLL.Services.Helper;
using NewsAggrigation.DAL.Models;
using NewsAggrigation.DAL.Repositories.ExternalApiRepo;

namespace NewsAggrigationService.Tests.ExternalApi
{
    public class ExternalApiTests
    {
        private readonly Mock<IExternalApiRepository> _mockRepo;
        private readonly ExternalApiService _service;

        public ExternalApiTests()
        {
            _mockRepo = new Mock<IExternalApiRepository>();
            _service = new ExternalApiService(_mockRepo.Object);
        }

        private ExternalAPIConfig CreateSampleConfig() =>
        new ExternalAPIConfig
        {
            ExternalAPIId = 1,
            Name = "NewsAPI",
            ApiUrl = "https://newsapi.org",
            ApiKey = "abc123",
            IsEnable = true,
            LastAccessedDate = DateTime.UtcNow
        };

        [Fact]
        public async Task GetAllAsync_ShouldReturnMappedResponses()
        {
            var configs = new List<ExternalAPIConfig> { CreateSampleConfig() };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(configs);

            var result = await _service.GetAllAsync();

            result.Should().HaveCount(1);
            result.First().ApiName.Should().Be("NewsAPI");
        }

        [Fact]
        public async Task GetAllAsync_ShouldThrowApiException_OnFailure()
        {
            _mockRepo.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("DB error"));

            var act = async () => await _service.GetAllAsync();

            await act.Should().ThrowAsync<ApiExceptionHelper>()
                .WithMessage("Failed to retrieve external API configs.");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnConfig_WhenExists()
        {
            var config = CreateSampleConfig();
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(config);

            var result = await _service.GetByIdAsync(1);

            result.Id.Should().Be(1);
            result.ApiName.Should().Be("NewsAPI");
        }
    }
}
