using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            try
            {
                var notifications = await _notificationService.GetUserNotificationsAsync(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to get notifications: {ex.Message}");
            }
        }

        [HttpGet("config")]
        public async Task<IActionResult> GetConfig()
        {
            try
            {
                var config = await _notificationService.GetUserNotificationConfigAsync(userId);
                return Ok(new { categories = config.Categories, keywords = config.Keywords });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to get notification config: {ex.Message}");
            }
        }

        [HttpPut("config/category")]
        public async Task<IActionResult> SetCategory([FromBody] CategoryConfigDto dto)
        {
            try
            {
                var result = await _notificationService.SetCategoryNotificationAsync(userId, dto.Category, dto.Enabled);
                if (!result) return BadRequest("Category not found.");
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to update category notification: {ex.Message}");
            }
        }

        [HttpPut("config/keywords")]
        public async Task<IActionResult> SetKeywords([FromBody] KeywordsConfigDto dto)
        {
            try
            {
                await _notificationService.SetKeywordNotificationsAsync(userId, dto.Keywords);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to update keyword notifications: {ex.Message}");
            }
        }
    }
}
