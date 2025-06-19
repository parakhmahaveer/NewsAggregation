using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewsAggrigation.DAL.Models;

namespace NewsAggrigation.DAL.Repositories.CategoryRepo
{
    public interface ICategoryRepository
    {
        //Task<Category?> GetAllCategoryAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<Category> AddCategoryAsync(Category category);
        Task<List<CategoryKeyword>> GetKeywordsByCategoryIdAsync(int categoryId);
        Task<bool> SoftDeleteCategoryAsync(int id);
        Task AddKeywordsAsync(int categoryId, IEnumerable<string> keywords);
        Task<bool> SoftDeleteKeywordAsync(int keywordId);
    }
}
