using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.API.ServiceDTOs.ResponseDTOs;
using NewsAggrigation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.BLL.Services.News
{
    public interface INewsService
    {
        Task<List<NewsResponse>> GetTodaysNewsAsync();
        Task<List<NewsResponse>> GetNewsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<NewsResponse>> GetTodaysNewsByCategoryAsync(NewsByCategoryRequest request);
        Task<List<Article>> SearchNewsAsync(string query, DateTime? startDate, DateTime? endDate, string? sortBy);
        Task SaveArticleAsync(string username, int articleId);
        Task<bool> UnsaveArticleAsync(string username, int articleId);
        Task<List<NewsResponse>> GetSavedArticlesAsync(string username);
    }
}
