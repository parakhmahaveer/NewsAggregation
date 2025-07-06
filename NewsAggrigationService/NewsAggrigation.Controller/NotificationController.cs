using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public NotificationController(INotificationService notificationService, IUserIdentityContext userIdentityContext)
        {
            _notificationService = notificationService;
            _userIdentityContext = userIdentityContext;
        }

        [HttpGet("config")]
        public async Task<IActionResult> GetConfig()
        {
            try
            {
                var config = await _notificationService.GetUserNotificationConfigAsync(_userIdentityContext.UserId);
                return Ok(new { categories = config.Categories, keywords = config.Keywords });
            }
            catch (ApiExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Unexpected error occurred: " + ex.Message });
            }
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetUserNotifications(string username)
        {
            try
            {
                var result = await _notificationService.GetUserNotificationsAsync(username);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to fetch notifications.", Details = ex.Message });
            }
        }

        [HttpPost("configure/category")]
        public async Task<IActionResult> ConfigureCategoryNotification([FromBody] ConfigureCategoryNotificationRequest request)
        {
            try
            {
                request.UserId = _userIdentityContext.UserId;
                await _notificationService.ConfigureCategoryNotificationAsync(request);
                return Ok(new { Message = "Category notification settings updated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to configure category notifications.", Details = ex.Message });
            }
        }

        [HttpPost("configure/keyword")]
        public async Task<IActionResult> ConfigureKeywordNotification([FromBody] ConfigureKeywordNotificationRequest request)
        {
            try
            {
                await _notificationService.ConfigureKeywordNotificationAsync(request);
                return Ok(new { Message = "Keyword notification settings updated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to configure keyword notifications.", Details = ex.Message });
            }
        }
    }
}
