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
        Task<List<NewsResponse>> SearchNewsAsync(SearchRequest request);
        Task SaveArticleAsync(string username, int articleId);
        Task<bool> UnsaveArticleAsync(string username, int articleId);
        Task<bool> SetArticleReactionAsync(ArticleReactionRequest request);
        Task<List<NewsResponse>> GetSavedArticlesAsync(string username);
        Task ReportArticleAsync(int articleId, string username);
        Task<List<NewsResponse>> GetReportedArticlesAsync();
        Task<bool> HideArticleAsync(int articleId);
        Task<bool> UnhideArticleAsync(int articleId);
    }
}
