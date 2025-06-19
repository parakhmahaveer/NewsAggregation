using Azure.Core;
using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.API.ServiceDTOs.ResponseDTOs;
using NewsAggrigation.DAL.Models;
using NewsAggrigation.DAL.Repositories.ExternalApiRepository;
using SendGrid.Helpers.Errors.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.BLL.Services.ExternalApi
{
    public class ExternalApiService : IExternalApiService
    {
        private readonly IExternalApiRepository _repository;

        public ExternalApiService(IExternalApiRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ExternalApiResponse>> GetAllAsync()
        {
            var configs = await _repository.GetAllAsync();
            return configs.Select(MapToResponse);
        }

        public async Task<ExternalApiResponse> GetByIdAsync(int id)
        {
            var config = await _repository.GetByIdAsync(id);
            return config is null ? null : MapToResponse(config);
        }

        public async Task<ExternalApiResponse> AddAsync(ExternalApiRequest request)
        {
            var newConfig = new ExternalAPIConfig
            {
                Name = request.ApiName,
                ApiUrl = request.BaseUrl,
                ApiKey = request.ApiKey
            };
            newConfig.IsEnable = true;
            var savedConfig = await _repository.AddAsync(newConfig);
            return MapToResponse(savedConfig);
        }

        public async Task<ExternalApiResponse> UpdateAsync(int id, ExternalApiUpdateRequest request)
        {
            var config = await _repository.GetByIdAsync(id) ?? throw new NotFoundException($"Config with ID {id} not found.");

            if (request.ApiName is not null)
                config.Name = request.ApiName;

            if (request.BaseUrl is not null)
                config.ApiUrl = request.BaseUrl;

            if (request.ApiKey is not null)
                config.ApiKey = request.ApiKey;

            var updatedConfig = await _repository.UpdateAsync(config);
            return MapToResponse(updatedConfig);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.SoftDeleteAsync(id);
        }

        private static ExternalApiResponse MapToResponse(ExternalAPIConfig config)
        {
            return new ExternalApiResponse
            {
                Id = config.ExternalAPIId,
                ApiName = config.Name,
                BaseUrl = config.ApiUrl,
                IsActive = config.IsEnable,
                LastAccessedAt = config.LastAccessedDate
            };
        }
    }
}
