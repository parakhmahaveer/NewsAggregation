using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.API.ServiceDTOs.ResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.BLL.Services.Catgory
{
    public interface ICategoryService
    {
        Task<IEnumerable<GetCategoriesResponse>> GetAllCategoriesAsync();
        Task<CategoryResponse> CreateCategoryAsync(string category);
        Task<CategoryResponse> GetCategoryWithKeywordsAsync(int categoryId);
        Task AddKeywordsAsync(int categoryId, CreateKeywordRequest request);
        Task DeleteCategoryAsync(int categoryId);
        Task DeleteKeywordAsync(int keywordId);
    }
}
