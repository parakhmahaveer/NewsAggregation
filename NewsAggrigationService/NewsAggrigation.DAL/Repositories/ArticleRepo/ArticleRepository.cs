using Microsoft.EntityFrameworkCore;
using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.API.ServiceDTOs.ResponseDTOs;
using NewsAggrigation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.DAL.Repositories.ArticleRepo
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly NewsAggregatorDbContext _context;
        public ArticleRepository(NewsAggregatorDbContext context)
        {
            _context = context;
        }
        public async Task<List<NewsResponse>> GetArticlesByDateRangeAsync(DateTime start, DateTime end)
        {
            return await _context.Articles
                .Where(a => a.PublishedDate >= start && a.PublishedDate <= end)
                .Select(a => new NewsResponse
                {
                    ArticleId = a.ArticleId,
                    Title = a.Title,
                    Content = a.Content,
                    Source = a.Source,
                    Url = a.Url,
                    Category = a.Category.CategoryName
                })
                .ToListAsync();
        }

        public async Task<List<NewsResponse>> GetArticlesByCategoryAndDateAsync(NewsByCategoryRequest request)
        {
            if (!DateTime.TryParse(request.StartDate, out var startDate) || !DateTime.TryParse(request.EndDate, out var endDate))
                throw new ArgumentException("Invalid date format.");

            var query = _context.Articles.AsQueryable();

            if (!string.Equals(request.Category, "all", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(a =>
                    a.Category.CategoryName.ToLower() == request.Category.ToLower());
                // OR string.Equals(..., ..., OrdinalIgnoreCase)
            }

            return await query
                .Where(a => a.PublishedDate >= startDate && a.PublishedDate < endDate.AddDays(1)) // inclusive
                .Select(a => new NewsResponse
                {
                    ArticleId = a.ArticleId,
                    Title = a.Title,
                    Url = a.Url,
                    Content = a.Content,
                    Source = a.Source,
                    Category = a.Category.CategoryName
                })
                .ToListAsync();
        }

        public async Task<List<Article>> SearchArticlesAsync(string query, DateTime? start, DateTime? end, string? sortBy)
        {
            var articlesQuery = _context.Articles.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                articlesQuery = articlesQuery.Where(a =>
                    a.Title.ToLower().Contains(query.ToLower()) ||
                    a.Content.ToLower().Contains(query.ToLower()));
            }

            if (start.HasValue)
            {
                articlesQuery = articlesQuery.Where(a => a.PublishedDate >= start.Value);
            }

            if (end.HasValue)
            {
                articlesQuery = articlesQuery.Where(a => a.PublishedDate <= end.Value);
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                articlesQuery = sortBy.ToLower() switch
                {
                    "title" => articlesQuery.OrderBy(a => a.Title),
                    "date" => articlesQuery.OrderBy(a => a.PublishedDate),
                    _ => articlesQuery
                };
            }

            return await articlesQuery.ToListAsync();
        }

        public async Task SaveArticleAsync(string username, int articleId)
        {
            var userId = (await _context.Users.FirstOrDefaultAsync(u => u.Username == username)).UserId;
            var articleExists = await _context.Articles.AnyAsync(a => a.ArticleId == articleId);
            if (!articleExists)
            {
                throw new ArgumentException("Article not found.");
            }

            var alreadySaved = await _context.SavedArticles.AnyAsync(sa => sa.UserId == userId && sa.ArticleId == articleId && !sa.IsDeleted);
            if (alreadySaved)
            {
                throw new InvalidOperationException("Article already saved by the user.");
            }

            var savedArticle = new SavedArticle
            {
                UserId = userId,
                ArticleId = articleId,
                SavedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.SavedArticles.Add(savedArticle);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteSavedArticleAsync(string username, int articleId)
        {
            var userId = (await _context.Users.FirstOrDefaultAsync(u => u.Username == username)).UserId;
            var saved = await _context.SavedArticles.FirstOrDefaultAsync(sa => sa.UserId == userId && sa.ArticleId == articleId && !sa.IsDeleted);
            if (saved == null) return false;
            saved.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<NewsResponse>> GetSavedArticlesByUserIdAsync(string username)
        {
            var userId = (await _context.Users.FirstOrDefaultAsync(u => u.Username == username)).UserId;
            return await _context.SavedArticles
                .Include(sa => sa.Article)
                .Where(sa => sa.UserId == userId && !sa.IsDeleted).Select(a => new NewsResponse
                {
                    ArticleId = a.Article.ArticleId,
                    Title = a.Article.Title,
                    Content = a.Article.Content,
                    Source = a.Article.Source,
                    Url = a.Article.Url,
                    Category = a.Article.Category.CategoryName
                })
                .ToListAsync();
        }
    }
}
