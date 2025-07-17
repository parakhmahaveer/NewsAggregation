using Moq;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using NewsAggrigation.BLL.Services.Catgory;
using NewsAggrigation.DAL.Repositories.CategoryRepo;
using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.BLL.Exceptions;
using NewsAggrigation.API.ServiceDTOs.ResponseDTOs;
using NewsAggrigation.DAL.Models;

namespace NewsAggrigationService.Tests.CategoryTests
{
    public class CategoryTests
    {
        private readonly Mock<ICategoryRepository> _mockRepo;
        private readonly CategoryService _service;

        public CategoryTests()
        {
            _mockRepo = new Mock<ICategoryRepository>();
            _service = new CategoryService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ShouldReturnAllCategories()
        {
            var categories = new List<GetCategoriesResponse>
            {
                new GetCategoriesResponse { CategoryId = 1, CategoryName = "Tech" },
                new GetCategoriesResponse { CategoryId = 2, CategoryName = "Health" }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

            var result = await _service.GetAllCategoriesAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetCategoryWithKeywordsAsync_ShouldThrow_WhenCategoryNotFound()
        {
            int categoryId = 1;
            _mockRepo.Setup(r => r.GetCategoryByIdAsync(categoryId))
                     .ReturnsAsync((Category?)null);

            var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetCategoryWithKeywordsAsync(categoryId));

            Assert.Equal($"Category with ID {categoryId} not found.", ex.Message);
        }

        [Fact]
        public async Task AddKeywordsAsync_ShouldThrow_WhenCategoryNotFound()
        {
            // Arrange
            int categoryId = 1;
            var request = new CreateKeywordRequest { CommaSeparatedKeywords = "one,two" };
            _mockRepo.Setup(r => r.GetCategoryByIdAsync(categoryId))
                     .ReturnsAsync((Category?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.AddKeywordsAsync(categoryId, request));

            Assert.Equal($"Category with ID {categoryId} not found.", ex.Message);
        }
    }
}
