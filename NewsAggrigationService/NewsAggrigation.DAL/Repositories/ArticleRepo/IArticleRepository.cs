using Azure.Core;
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
    public interface IArticleRepository
    {
        Task<List<NewsResponse>> GetArticlesByDateRangeAsync(DateTime start, DateTime end);
        Task<List<NewsResponse>> GetArticlesByCategoryAndDateAsync(NewsByCategoryRequest request);
        Task<List<NewsResponse>> SearchArticlesAsync(SearchRequest request);
        Task SaveArticleAsync(string username, int articleId);
        Task<bool> DeleteSavedArticleAsync(string username, int articleId);
        Task<List<NewsResponse>> GetSavedArticlesByUserIdAsync(string username);
        Task<bool> SetArticleReactionByArticleIdAsync(ArticleReactionRequest request);
    }
}
