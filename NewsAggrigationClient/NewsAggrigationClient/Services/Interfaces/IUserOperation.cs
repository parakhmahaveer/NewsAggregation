using NewsAggrigationClient.Models.DTOs.RequestDTOs;
using NewsAggrigationClient.Models.DTOs.ResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigationClient.Services.Interfaces
{
    public interface IUserOperation
    {
        Task ViewTodaysNewsAsync();
        Task SaveArticleAsync();
        Task DeleteSavedArticleAsync();
        Task ViewSavedArticlesAsync();
        Task<List<GetCategoriesResponse>> GetAllCategoriesAsync();
        Task ReactToArticleAsync();
        Task SearchArticlesAsync();
        Task ReportArticleAsync();
        Task ViewNotificationsAsync();
        Task ViewRecommendedArticlesAsync();
        Task SetCategoryNotificationAsync(string category, bool enabled);
        Task SetKeywordNotificationsAsync(List<string> keywords);
        Task ViewHeadlinesAsync(NewsByCategoryRequest request);
    }
}
