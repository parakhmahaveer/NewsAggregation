using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.BLL.Services.Categorizer
{
    public interface ICategorizerService
    {
        Task<int?> DetectCategoryAsync(string content);
    }
}
