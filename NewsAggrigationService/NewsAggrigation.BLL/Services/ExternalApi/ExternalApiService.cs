using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.API.ServiceDTOs.ResponseDTOs;
using NewsAggrigation.BLL.Exceptions;
using NewsAggrigation.BLL.Services.Helper;
using NewsAggrigation.DAL.Models;
using NewsAggrigation.DAL.Repositories.ExternalApiRepo;


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
            try
            {
                var configs = await _repository.GetAllAsync();
                return configs.Select(MapToResponse);
            }
            catch (Exception ex)
            {
                throw new ApiExceptionHelper("Failed to retrieve external API configs.", 500);
            }
        }

        public async Task<ExternalApiResponse> GetByIdAsync(int id)
        {
            try
            {
                var config = await _repository.GetByIdAsync(id);
                if (config is null)
                    throw new NotFoundException($"Config with ID {id} not found.");
                return MapToResponse(config);
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiExceptionHelper("Failed to retrieve external API config.", 500);
            }
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

        public async Task<ExternalApiResponse> UpdateExternalApiAsync(ExternalApiUpdateRequest request)
        {
            var config = await _repository.GetByIdAsync(request.ApiId) ?? throw new NotFoundException($"Config with ID {request.ApiId} not found.");

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
                ApiKey = config.ApiKey,
                IsActive = config.IsEnable,
                LastAccessedAt = config.LastAccessedDate
            };
        }
    }
}
