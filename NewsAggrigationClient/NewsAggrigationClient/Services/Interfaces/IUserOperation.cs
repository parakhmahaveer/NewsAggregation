using NewsAggrigationClient.Models.DTOs.RequestDTOs;
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
        Task ReactToArticleAsync();
        Task SearchArticlesAsync();
        Task ViewHeadlinesAsync(NewsByCategoryRequest request);
    }
}
