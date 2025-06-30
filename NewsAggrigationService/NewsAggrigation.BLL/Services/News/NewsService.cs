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
        public NewsService (IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task<List<NewsResponse>> GetTodaysNewsAsync()
        {
            try
            {
                var todayStartTime = DateTime.UtcNow.Date;
                var todayEndTime = DateTime.UtcNow.Date.Add(TimeSpan.FromDays(1));
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
    }
}
