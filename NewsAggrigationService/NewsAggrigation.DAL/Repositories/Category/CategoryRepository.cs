using Microsoft.EntityFrameworkCore;
using NewsAggrigation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.DAL.Repositories.CategoryRepo
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly NewsAggregatorDbContext _context;

        public CategoryRepository(NewsAggregatorDbContext context)
        {
            _context = context;
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id && !c.IsDeleted);
        }

        public async Task<bool> SoftDeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null || category.IsDeleted) return false;

            category.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task AddKeywordsAsync(int categoryId, IEnumerable<string> keywords)
        {
            var keywordEntities = keywords.Select(k => new CategoryKeyword
            {
                CategoryId = categoryId,
                KeywordName = k.Trim(),
                IsDeleted = false
            });

            await _context.CategoryKeywords.AddRangeAsync(keywordEntities);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteKeywordAsync(int keywordId)
        {
            var keyword = await _context.Keywords.FindAsync(keywordId);
            if (keyword == null || keyword.IsDeleted) return false;

            keyword.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CategoryKeyword>> GetKeywordsByCategoryIdAsync(int categoryId)
        {
            return await _context.CategoryKeywords
                .Where(k => k.CategoryId == categoryId && !k.IsDeleted)
                .ToListAsync();
        }
    }
}
