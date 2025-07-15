using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.BLL.Services.Helper;
using NewsAggrigation.BLL.Services.Helper.UserIdentity;
using NewsAggrigation.BLL.Services.News;

namespace NewsAggrigation.Controller
{
    [ApiController]
    [Route("api/news")]
    [Authorize]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        private readonly IUserIdentityContext _userIdentityContext;
        private readonly ILogger<NewsController> _logger;

        public NewsController(INewsService newsService, IUserIdentityContext userIdentityContext, ILogger<NewsController> logger)
        {
            _newsService = newsService;
            _userIdentityContext = userIdentityContext;
            _logger = logger;
        }

        [HttpGet("today")]
        public async Task<IActionResult> GetTodaysNews()
        {
            try
            {
                _logger.LogInformation("Fetching today's news articles.");
                var articles = await _newsService.GetTodaysNewsAsync();
                return Ok(articles);
            }
            catch (ApiExceptionHelper ex)
            {
                _logger.LogWarning(ex, "API exception occurred while fetching today's news.");
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching today's news.");
                return StatusCode(500, new { Message = "Unexpected error occurred." + ex.Message });
            }
        }

        [HttpGet("date-range")]
        public async Task<IActionResult> GetNewsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                _logger.LogInformation("Fetching news from {StartDate} to {EndDate}.", startDate, endDate);
                var articles = await _newsService.GetNewsByDateRangeAsync(startDate, endDate);
                return Ok(articles);
            }
            catch (ApiExceptionHelper ex)
            {
                _logger.LogWarning(ex, "API exception occurred while fetching news by date range.");
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching news by date range.");
                return StatusCode(500, new { Message = "Unexpected error occurred." + ex.Message });
            }
        }

        [HttpPost("category")]
        public async Task<IActionResult> GetTodaysNewsByCategory([FromBody] NewsByCategoryRequest request)
        {
            try
            {
                _logger.LogInformation("Fetching news by category: {Category}", request.Category);
                var articles = await _newsService.GetTodaysNewsByCategoryAsync(request);
                return Ok(articles);
            }
            catch (ApiExceptionHelper ex)
            {
                _logger.LogWarning(ex, "API exception occurred while fetching news by category.");
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching news by category.");
                return StatusCode(500, new { Message = "Unexpected error occurred." + ex.Message });
            }
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchNews([FromBody] SearchRequest request)
        {
            try
            {
                _logger.LogInformation("Searching news with query: {Query}", request.Query);
                var articles = await _newsService.SearchNewsAsync(request);
                return Ok(articles);
            }
            catch (ApiExceptionHelper ex)
            {
                _logger.LogWarning(ex, "API exception occurred while searching news.");
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while searching news.");
                return StatusCode(500, new { Message = "Unexpected error occurred." + ex.Message });
            }
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveArticle([FromQuery] string username, [FromQuery] int articleId)
        {
            try
            {
                _logger.LogInformation("Saving article {ArticleId} for user {Username}.", articleId, username);
                await _newsService.SaveArticleAsync(username, articleId);
                return Ok(new { Message = "Article saved successfully." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Article not found while saving.");
                return NotFound(new { ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Article already saved.");
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while saving article.");
                return StatusCode(500, new { Message = "Unexpected error occurred." + ex.Message });
            }
        }

        [HttpPost("react")]
        public async Task<IActionResult> SetArticleReaction([FromBody] ArticleReactionRequest request)
        {
            try
            {
                request.UserId = _userIdentityContext.UserId;
                _logger.LogInformation("User {UserId} reacting to article {ArticleId}.", request.UserId, request.ArticleId);
                var result = await _newsService.SetArticleReactionAsync(request);
                if (!result)
                    return BadRequest(new { Message = "Could not update feedback." });

                return Ok(new { Message = request.IsLiked ? "Article liked." : "Article disliked." });
            }
            catch (ApiExceptionHelper ex)
            {
                _logger.LogWarning(ex, "API exception occurred while reacting to article.");
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while reacting to article.");
                return StatusCode(500, new { Message = "Unexpected error occurred." + ex.Message });
            }
        }

        [HttpDelete("unsave")]
        public async Task<IActionResult> UnsaveArticle([FromQuery] string username, [FromQuery] int articleId)
        {
            try
            {
                _logger.LogInformation("Un-saving article {ArticleId} for user {Username}.", articleId, username);
                var result = await _newsService.UnsaveArticleAsync(username, articleId);
                return result ? NoContent() : NotFound(new { Message = "Saved article not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while un-saving article.");
                return StatusCode(500, new { Message = "Unexpected error occurred." + ex.Message });
            }
        }

        [HttpGet("saved/{username}")]
        public async Task<IActionResult> GetSavedArticles(string username)
        {
            try
            {
                _logger.LogInformation("Fetching saved articles for user {Username}.", username);
                var articles = await _newsService.GetSavedArticlesAsync(username);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching saved articles.");
                return StatusCode(500, new { Message = "Unexpected error occurred." + ex.Message });
            }
        }

        [HttpPost("{articleId}/report")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ReportArticle(int articleId)
        {
            try
            {
                var userId = _userIdentityContext.UserId;
                _logger.LogInformation("User {UserId} reporting article {ArticleId}.", userId, articleId);
                await _newsService.ReportArticleAsync(articleId, userId);
                return Ok(new { Message = "Article reported successfully." });
            }
            catch (ApiExceptionHelper ex)
            {
                _logger.LogWarning(ex, "API exception occurred while reporting article.");
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while reporting article.");
                return StatusCode(500, new { Message = "Error reporting article.", Details = ex.Message });
            }
        }

        [HttpGet("reported")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetReportedArticles()
        {
            try
            {
                _logger.LogInformation("Fetching reported articles.");
                var articles = await _newsService.GetReportedArticlesAsync();
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve reported articles.");
                return StatusCode(500, new { Message = "Failed to retrieve reported articles." + ex.Message });
            }
        }

        [HttpPost("{articleId}/hide")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> HideArticle(int articleId)
        {
            try
            {
                _logger.LogInformation("Hiding article {ArticleId}.", articleId);
                var success = await _newsService.HideArticleAsync(articleId);
                return success ? Ok(new { Message = "Article hidden." }) : NotFound(new { Message = "Article not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to hide article.");
                return StatusCode(500, new { Message = "Failed to hide article." + ex.Message });
            }
        }

        [HttpPost("{articleId}/unhide")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnhideArticle(int articleId)
        {
            try
            {
                _logger.LogInformation("Unhiding article {ArticleId}.", articleId);
                var success = await _newsService.UnhideArticleAsync(articleId);
                return success ? Ok(new { Message = "Article unhidden." }) : NotFound(new { Message = "Article not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unhide article.");
                return StatusCode(500, new { Message = "Failed to unhide article." + ex.Message });
            }
        }

        [HttpGet("personalized")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetPersonalizedArticles()
        {
            try
            {
                var userId = _userIdentityContext.UserId;
                _logger.LogInformation("Fetching personalized articles for user {UserId}.", userId);
                var articles = await _newsService.GetPersonalizedArticlesAsync(userId);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch personalized articles.");
                return StatusCode(500, new { Message = "Failed to fetch personalized articles." + ex.Message });
            }
        }
    }
}
