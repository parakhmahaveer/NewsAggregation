using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.BLL.Services.Helper;
using NewsAggrigation.BLL.Services.Helper.UserIdentity;
using NewsAggrigation.BLL.Services.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.Controller
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IUserIdentityContext _userIdentityContext;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(INotificationService notificationService, IUserIdentityContext userIdentityContext, ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _userIdentityContext = userIdentityContext;
            _logger = logger;
        }

        [HttpGet("config")]
        public async Task<IActionResult> GetNotificationConfig()
        {
            try
            {
                _logger.LogInformation("Getting notification config for user {UserId}", _userIdentityContext.UserId);
                var notificationConfig = await _notificationService.GetUserNotificationConfigAsync(_userIdentityContext.UserId);
                _logger.LogInformation("Successfully retrieved notification config for user {UserId}", _userIdentityContext.UserId);
                return Ok(notificationConfig);
            }
            catch (ApiExceptionHelper ex)
            {
                _logger.LogWarning(ex, "API exception while getting notification config for user {UserId}", _userIdentityContext.UserId);
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while getting notification config for user {UserId}", _userIdentityContext.UserId);
                return StatusCode(500, new { Message = "Unexpected error occurred: " + ex.Message });
            }
        }

        [HttpGet()]
        public async Task<IActionResult> GetUserNotifications()
        {
            try
            {
                _logger.LogInformation("Getting notifications for user {UserId}", _userIdentityContext.UserId);
                var result = await _notificationService.GetUserNotificationsAsync(_userIdentityContext.UserId);
                _logger.LogInformation("Successfully retrieved notifications for user {UserId}", _userIdentityContext.UserId);
                return Ok(result);
            }
            catch (ApiExceptionHelper ex)
            {
                _logger.LogWarning(ex, "API exception while getting notifications for user {UserId}", _userIdentityContext.UserId);
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while getting notifications for user {UserId}", _userIdentityContext.UserId);
                return StatusCode(500, new { Message = "Unexpected error occurred: " + ex.Message });
            }
        }

        [HttpPost("configure/category")]
        public async Task<IActionResult> ConfigureCategoryNotification([FromBody] ConfigureCategoryNotificationRequest request)
        {
            try
            {
                _logger.LogInformation("Configuring category notification for user {UserId}", _userIdentityContext.UserId);
                request.UserId = _userIdentityContext.UserId;
                await _notificationService.ConfigureCategoryNotificationAsync(request);
                _logger.LogInformation("Category notification settings updated for user {UserId}", _userIdentityContext.UserId);
                return Ok(new { Message = "Category notification settings updated." });
            }
            catch (ApiExceptionHelper ex)
            {
                _logger.LogWarning(ex, "API exception while configuring category notification for user {UserId}", _userIdentityContext.UserId);
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while configuring category notification for user {UserId}", _userIdentityContext.UserId);
                return StatusCode(500, new { Message = "Unexpected error occurred: " + ex.Message });
            }
        }

        [HttpPost("configure/keyword")]
        public async Task<IActionResult> ConfigureKeywordNotification([FromBody] ConfigureKeywordNotificationRequest request)
        {
            try
            {
                _logger.LogInformation("Configuring keyword notification for user {UserId}", _userIdentityContext.UserId);
                request.UserId = _userIdentityContext.UserId;
                await _notificationService.ConfigureKeywordNotificationAsync(request);
                _logger.LogInformation("Keyword notification settings updated for user {UserId}", _userIdentityContext.UserId);
                return Ok(new { Message = "Keyword notification settings updated." });
            }
            catch (ApiExceptionHelper ex)
            {
                _logger.LogWarning(ex, "API exception while configuring keyword notification for user {UserId}", _userIdentityContext.UserId);
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while configuring keyword notification for user {UserId}", _userIdentityContext.UserId);
                return StatusCode(500, new { Message = "Unexpected error occurred: " + ex.Message });
            }
        }
    }
}
