using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.API.ServiceDTOs.ResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.BLL.Services.ExternalApi
{
    public interface IExternalApiService
    {
        Task<IEnumerable<ExternalApiResponse>> GetAllAsync();
        Task<ExternalApiResponse> GetByIdAsync(int id);
        Task<ExternalApiResponse> AddAsync(ExternalApiRequest dto);
        Task<ExternalApiResponse> UpdateAsync(int id, ExternalApiUpdateRequest dto);
        Task<bool> DeleteAsync(int id);
    }
}
