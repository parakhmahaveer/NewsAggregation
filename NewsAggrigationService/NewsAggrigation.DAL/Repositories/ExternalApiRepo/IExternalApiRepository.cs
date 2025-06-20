using NewsAggrigation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.DAL.Repositories.ExternalApiRepo
{
    public interface IExternalApiRepository
    {
        Task<IEnumerable<ExternalAPIConfig>> GetAllAsync();
        Task<ExternalAPIConfig> GetByIdAsync(int id);
        Task<ExternalAPIConfig> AddAsync(ExternalAPIConfig config);
        Task<ExternalAPIConfig> UpdateAsync(ExternalAPIConfig config);
        Task<bool> SoftDeleteAsync(int id);
    }
}
