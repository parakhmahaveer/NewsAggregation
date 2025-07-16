using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.API.ServiceDTOs.ResponseDTOs;
using NewsAggrigation.DAL.Models;
using NewsAggrigation.DAL.Repositories.CategoryRepo;
using SendGrid.Helpers.Errors.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.BLL.Services.Catgory
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<GetCategoriesResponse>> GetAllCategoriesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<CategoryResponse> CreateCategoryAsync(string categoryName)
        {
            var existingCategories = await _repository.GetAllAsync();
            if (existingCategories.Any(c =>
                string.Equals(c.CategoryName, categoryName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Category '{categoryName}' already exists.");
            }

            var category = new Category
            {
                CategoryName = categoryName,
                IsDeleted = false
            };

            var saved = await _repository.AddCategoryAsync(category);

            return new CategoryResponse
            {
                CategoryId = saved.CategoryId,
                CategoryName = saved.CategoryName,
                Keywords = new List<string>()
            };
        }

        public async Task<CategoryResponse> GetCategoryWithKeywordsAsync(int categoryId)
        {
            var category = await _repository.GetCategoryByIdAsync(categoryId)
                ?? throw new NotFoundException($"Category with ID {categoryId} not found.");

            var keywords = await _repository.GetKeywordsByCategoryIdAsync(categoryId);

            return new CategoryResponse
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Keywords = keywords.Select(k => k.KeywordName).ToList()
            };
        }

        public async Task AddKeywordsAsync(int categoryId, CreateKeywordRequest request)
        {
            var category = await _repository.GetCategoryByIdAsync(categoryId)
                ?? throw new NotFoundException($"Category with ID {categoryId} not found.");

            var keywords = request.CommaSeparatedKeywords
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(k => !string.IsNullOrWhiteSpace(k));

            await _repository.AddKeywordsAsync(categoryId, keywords);
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            var result = await _repository.DeleteCategoryAsync(categoryId);
            if (!result)
                throw new NotFoundException($"Category with ID {categoryId} not found.");
        }

        public async Task DeleteKeywordAsync(int keywordId)
        {
            var result = await _repository.DeleteKeywordAsync(keywordId);
            if (!result)
                throw new NotFoundException($"Keyword with ID {keywordId} not found.");
        }

        public async Task<bool> HideCategoryAsync(int categoryId)
        {
            var category = await _repository.GetByIdAsync(categoryId);
            if (category == null) return false;

            await _repository.HideCategoryAsync(category);
            return true;
        }

        public async Task<bool> UnhideCategoryAsync(int categoryId)
        {
            var category = await _repository.GetByIdAsync(categoryId);
            if (category == null) return false;

            await _repository.UnhideCategoryAsync(category);
            return true;
        }

        public async Task<int> BlockArticlesByKeywordAsync(string keyword)
        {
            return await _repository.BlockArticlesByKeywordAsync(keyword);
        }
    }
}
