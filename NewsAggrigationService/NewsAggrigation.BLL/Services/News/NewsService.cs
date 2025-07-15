using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.API.ServiceDTOs.ResponseDTOs;
using NewsAggrigation.DAL.Models;
using NewsAggrigation.DAL.Repositories.ArticleRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.BLL.Services.News
{
    public class NewsService : INewsService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IConfiguration _configuration;

        public NewsService (IArticleRepository articleRepository, IConfiguration configuration)
        {
            _articleRepository = articleRepository;
            _configuration = configuration;
        }

        public async Task<List<NewsResponse>> GetTodaysNewsAsync()
        {
            try
            {
                var todayStartTime = DateTime.Now.Date;
                var todayEndTime = DateTime.Now.Date.Add(TimeSpan.FromDays(1));
                return await _articleRepository.GetArticlesByDateRangeAsync(todayStartTime, todayEndTime);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get today's news.", ex);
            }
        }

        public async Task<List<NewsResponse>> GetNewsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _articleRepository.GetArticlesByDateRangeAsync(startDate.Date, endDate.Date);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get news by date range.", ex);
            }
        }

        public async Task<List<NewsResponse>> GetTodaysNewsByCategoryAsync(NewsByCategoryRequest request)
        {
            try
            {
                return await _articleRepository.GetArticlesByCategoryAndDateAsync(request);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get today's news by category.", ex);
            }
        }

        public async Task<List<NewsResponse>> SearchNewsAsync(SearchRequest request)
        {
            try
            {
                return await _articleRepository.SearchArticlesAsync(request);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to search news.", ex);
            }
        }

        public async Task SaveArticleAsync(string username, int articleId)
        {
            await _articleRepository.SaveArticleAsync(username, articleId);
        }

        public async Task<bool> UnsaveArticleAsync(string username, int articleId)
        {
            return await _articleRepository.DeleteSavedArticleAsync(username, articleId);
        }

        public async Task<List<NewsResponse>> GetSavedArticlesAsync(string username)
        {
            return await _articleRepository.GetSavedArticlesByUserIdAsync(username);
        }

        public async Task<bool> SetArticleReactionAsync(ArticleReactionRequest request)
        {
            return await _articleRepository.SetArticleReactionByArticleIdAsync(request);
        }

        public async Task ReportArticleAsync(int articleId, int userId)
        {
            var article = await _articleRepository.GetByIdAsync(articleId)
                    ?? throw new ArgumentException("Article not found");

            var alreadyReported = await _articleRepository.HasUserReportedAsync(articleId, userId);
            if (alreadyReported)
            {
                throw new InvalidOperationException("You’ve already reported this article.");
            }

            await _articleRepository.ReportArticleAsync(articleId, userId);

            int reportCount = await _articleRepository.GetReportCountAsync(articleId);
            int autoHideThresholdCount = Convert.ToInt32(_configuration["AutoHideThresholdCount"]);

            if (reportCount >= autoHideThresholdCount)
            {
                await _articleRepository.HideArticleAsync(article);
            }
        }

        public async Task<List<NewsResponse>> GetReportedArticlesAsync()
        {
            var reported = await _articleRepository.GetReportedArticlesAsync();

            return reported.Select(a => new NewsResponse
            {
                ArticleId = a.ArticleId,
                Title = a.Title,
                Content = a.Content,
                Url = a.Url,
                Source = a.Source,
                Category = a.Category.CategoryName
            }).ToList();
        }

        public async Task<bool> HideArticleAsync(int articleId)
        {
            var article = await _articleRepository.GetByIdAsync(articleId);
            if (article == null) return false;

            await _articleRepository.HideArticleAsync(article);
            return true;
        }

        public async Task<bool> UnhideArticleAsync(int articleId)
        {
            var article = await _articleRepository.GetByIdAsync(articleId);
            if (article == null) return false;

            await _articleRepository.UnhideArticleAsync(article);
            return true;
        }

        public async Task<IEnumerable<NewsResponse>> GetPersonalizedArticlesAsync(int userId)
        {
            var articles = await _articleRepository.GetRecommendedArticlesForTodayAsync(userId);

            return articles.Select(a => new NewsResponse
            {
                ArticleId = a.ArticleId,
                Title = a.Title,
                Url = a.Url,
                Source = a.Source
            });
        }
    }
}
