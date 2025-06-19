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
        Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request);
        Task<CategoryResponse> GetCategoryWithKeywordsAsync(int categoryId);
        Task AddKeywordsAsync(int categoryId, CreateKeywordRequest request);
        Task SoftDeleteCategoryAsync(int categoryId);
        Task SoftDeleteKeywordAsync(int keywordId);
    }
}
