using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigationClient.Services.Interfaces
{
    public interface IAdminOperation
    {
        Task ViewAllExternalServersAsync();
        Task ViewExternalServerDetailsAsync();
        Task AddCategoryAsync();
        Task UpdateExternalServerAsync();
        Task ViewReportedArticlesAsync();
        Task ToggleArticleVisibilityAsync();
        Task ToggleCategoryVisibilityAsync();
        Task ToggleKeywordVisibilityAsync();
        Task BlockArticlesByKeywordAsync();
    }
}
