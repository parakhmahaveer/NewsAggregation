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

        public async Task<List<NewsResponse>> SearchArticlesAsync(SearchRequest request)
        {
            if (!DateTime.TryParse(request.StartDate, out var startDate) || !DateTime.TryParse(request.EndDate, out var endDate))
            {
                throw new ArgumentException("Invalid date range.");
            }

            var query = _context.Articles
                .Where(a =>
                    !a.IsDeleted &&
                    a.PublishedDate >= startDate &&
                    a.PublishedDate <= endDate &&
                    (a.Title.Contains(request.Query) || a.Content.Contains(request.Query)));

            if (request.SortBy?.ToLower() == "likes")
            {
                query = query.OrderByDescending(a => a.LikeCount);
            }
            else if (request.SortBy?.ToLower() == "dislikes")
            {
                query = query.OrderByDescending(a => a.DisLikeCount);
            }

            return await query.Select(a => new NewsResponse
            {
                ArticleId = a.ArticleId,
                Title = a.Title,
                Url = a.Url,
                Source = a.Source,
                Category = a.Category.CategoryName,
                LikeCount = a.LikeCount,
                DislikeCount = a.DisLikeCount
            }).ToListAsync();
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
                SavedDate = DateTime.Now,
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

        public async Task<bool> SetArticleReactionByArticleIdAsync(ArticleReactionRequest request)
        {
            var userId = (await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username)).UserId;
            var article = await _context.Articles.FirstOrDefaultAsync(a => a.ArticleId == request.ArticleId);
            if (article != null)
            {
                if (request.IsLiked)
                {
                    article.LikeCount++;
                }
                else
                {
                    article.DisLikeCount++;
                }
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<Article>> GetReportedArticlesAsync()
        {
            var reportedArticleIds = await _context.ReportedArticles.
                Select(ra => ra.ArticleId).Distinct().ToListAsync();

            var articles = await _context.Articles
                .Where(a => reportedArticleIds.Contains(a.ArticleId)).Include(a => a.Category).ToListAsync();

            return articles;
        }

        public async Task<bool> HasUserReportedAsync(int articleId, int userId)
        {
            return await _context.UserArticleActivity.AnyAsync(a => a.ArticleId == articleId && a.UserId == userId && a.IsFlagged);
        }

        public async Task ReportArticleAsync(int articleId, int userId)
        {
            var reportedArticle = await _context.ReportedArticles.FirstOrDefaultAsync(r => r.ArticleId == articleId);

            if (reportedArticle != null)
            {
                reportedArticle.ReportCount++;
            }
            else
            {
                reportedArticle = new ReportedArticle
                {
                    ArticleId = articleId,
                    ReportCount = 1
                };
                await _context.ReportedArticles.AddAsync(reportedArticle);
            }

            var userArticleActivity = new UserArticleActivity
            {
                ArticleId = articleId,
                IsFlagged = true,
                UserId = userId
            };
            _context.UserArticleActivity.Add(userArticleActivity);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetReportCountAsync(int articleId)
        {
            var reportedArticle = await _context.ReportedArticles.FirstOrDefaultAsync(r => r.ArticleId == articleId);
            return reportedArticle.ReportCount;
        }

        public async Task HideArticleAsync(Article article)
        {
            article.IsDeleted = true;
            _context.Articles.Update(article);
            await _context.SaveChangesAsync();
        }

        public async Task UnhideArticleAsync(Article article)
        {
            article.IsDeleted = false;
            _context.Articles.Update(article);
            await _context.SaveChangesAsync();
        }

        public async Task<Article?> GetByIdAsync(int articleId)
        {
            return await _context.Articles.FirstOrDefaultAsync(a => a.ArticleId == articleId);
        }

        public async Task<IEnumerable<Article>> GetRecommendedArticlesForTodayAsync(int userId)
        {
            var today = DateTime.Now.Date;

            // Get liked articles
            var likedArticleIds = await _context.UserArticleActivity
                .Where(x => x.UserId == userId && x.IsLiked)
                .Select(x => x.ArticleId)
                .ToListAsync();

            // Get saved articles
            var savedArticleIds = await _context.SavedArticles
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .Select(x => x.ArticleId)
                .ToListAsync();

            // Get enabled category IDs
            var enabledCategoryIds = await _context.CategoryNotificationSettings
                .Where(x => x.UserId == userId && x.IsEnabled && !x.IsDeleted)
                .Select(x => x.CategoryId)
                .ToListAsync();

            // Get enabled keywords
            var enabledKeywordList = await _context.Keywords
                .Where(k => !k.IsDeleted && k.IsEnabled && k.UserId == userId)
                .Select(k => k.Word)
                .ToListAsync();

            // Build filtered article queries
            var articlesFromLikesOrSaves = _context.Articles
                .Where(a =>
                    (likedArticleIds.Contains(a.ArticleId) || savedArticleIds.Contains(a.ArticleId)) &&
                    a.PublishedDate.Date == today &&
                    !a.IsDeleted);

            var articlesFromCategories = _context.Articles
                .Where(a =>
                    enabledCategoryIds.Contains(a.CategoryId) &&
                    a.PublishedDate.Date == today &&
                    !a.IsDeleted);

            var articlesFromKeywords = _context.Articles
                .Where(a =>
                    a.PublishedDate.Date == today &&
                    !a.IsDeleted &&
                    enabledKeywordList.Any(k => a.Title.Contains(k) || a.Content.Contains(k)));

            // Combine all and return distinct results
            var recommendedArticles = await articlesFromLikesOrSaves
                .Union(articlesFromCategories)
                .Union(articlesFromKeywords)
                .Distinct()
                .OrderByDescending(a => a.PublishedDate)
                .ToListAsync();

            return recommendedArticles;
        }
    }
}
