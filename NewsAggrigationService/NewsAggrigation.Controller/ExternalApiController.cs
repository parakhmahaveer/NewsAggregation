using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.BLL.Exceptions;
using NewsAggrigation.BLL.Services.ExternalApi;
using NewsAggrigation.BLL.Services.Helper;

namespace NewsAggrigation.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ExternalApisController : ControllerBase
    {
        private readonly IExternalApiService _externalApiService;

        public ExternalApisController(IExternalApiService service)
        {
            _externalApiService = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllExternalApisAsync()
        {
            try
            {
                var apis = await _externalApiService.GetAllAsync();
                return Ok(apis);
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized();
            }
            catch (ApiExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred while retrieving APIs. " + ex.Message });
            }
        }

        [HttpGet("{apiId}")]
        public async Task<IActionResult> GetExternalApiByIdAsync(int apiId)
        {
            try
            {
                var api = await _externalApiService.GetByIdAsync(apiId);
                return Ok(api);
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex.Message });
            }
            catch (ApiExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred while retrieving the API. " + ex.Message });
            }
        }

        [HttpPatch("update")]
        public async Task<IActionResult> UpdateExternalApiAsync([FromBody] ExternalApiUpdateRequest request)
        {
            try
            {
                var updatedApi = await _externalApiService.UpdateExternalApiAsync(request);
                return Ok(updatedApi);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex.Message });
            }
            catch (ApiExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating the external API. " + ex.Message });
            }
        }

        [HttpDelete("{apiId}")]
        public async Task<IActionResult> DeleteExternalApiAsync(int apiId)
        {
            try
            {
                var isDeleted = await _externalApiService.DeleteAsync(apiId);
                return isDeleted ? NoContent() : NotFound(new { Message = "External API not found." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex.Message });
            }
            catch (ApiExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting the external API. " + ex.Message });
            }
        }
    }
}
