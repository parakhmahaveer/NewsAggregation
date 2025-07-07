using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public NewsController(INewsService newsService, IUserIdentityContext userIdentityContext)
        {
            _newsService = newsService;
            _userIdentityContext = userIdentityContext;
        }

        [HttpGet("today")]
        public async Task<IActionResult> GetTodaysNews()
        {
            try
            {
                var articles = await _newsService.GetTodaysNewsAsync();
                return Ok(articles);
            }
            catch (ApiExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Unexpected error occurred." + ex.Message });
            }
        }

        [HttpGet("date-range")]
        public async Task<IActionResult> GetNewsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var articles = await _newsService.GetNewsByDateRangeAsync(startDate, endDate);
                return Ok(articles);
            }
            catch (ApiExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Unexpected error occurred." + ex.Message });
            }
        }

        [HttpPost("category")]
        public async Task<IActionResult> GetTodaysNewsByCategory([FromBody] NewsByCategoryRequest request)
        {
            try
            {
                var articles = await _newsService.GetTodaysNewsByCategoryAsync(request);
                return Ok(articles);
            }
            catch (ApiExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Unexpected error occurred." + ex.Message });
            }
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchNews([FromBody] SearchRequest request)
        {
            try
            {
                var articles = await _newsService.SearchNewsAsync(request);
                return Ok(articles);
            }
            catch (ApiExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Unexpected error occurred." + ex.Message });
            }
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveArticle([FromQuery] string username, [FromQuery] int articleId)
        {
            try
            {
                await _newsService.SaveArticleAsync(username, articleId);
                return Ok(new { Message = "Article saved successfully." });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Unexpected error occurred." + ex.Message });
            }
        }

        [HttpPost("react")]
        public async Task<IActionResult> SetArticleReaction([FromBody] ArticleReactionRequest request)
        {
            try
            {
                var result = await _newsService.SetArticleReactionAsync(request);
                if (!result)
                    return BadRequest(new { Message = "Could not update feedback." });

                return Ok(new { Message = request.IsLiked ? "Article liked." : "Article disliked." });
            }
            catch (ApiExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Unexpected error occurred." + ex.Message });
            }
        }

        [HttpDelete("unsave")]
        public async Task<IActionResult> UnsaveArticle([FromQuery] string username, [FromQuery] int articleId)
        {
            try
            {
                var result = await _newsService.UnsaveArticleAsync(username, articleId);
                return result ? NoContent() : NotFound(new { Message = "Saved article not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Unexpected error occurred." + ex.Message });
            }
        }

        [HttpGet("saved/{username}")]
        public async Task<IActionResult> GetSavedArticles(string username)
        {
            try
            {
                var articles = await _newsService.GetSavedArticlesAsync(username);
                return Ok(articles);
            }
            catch (Exception ex)
            {
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
                await _newsService.ReportArticleAsync(articleId, userId);
                return Ok(new { Message = "Article reported successfully." });
            }
            catch (ApiExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error reporting article.", Details = ex.Message });
            }
        }

        [HttpGet("reported")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetReportedArticles()
        {
            try
            {
                var articles = await _newsService.GetReportedArticlesAsync();
                return Ok(articles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to retrieve reported articles." + ex.Message });
            }
        }

        [HttpPost("{articleId}/hide")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> HideArticle(int articleId)
        {
            try
            {
                var success = await _newsService.HideArticleAsync(articleId);
                return success ? Ok(new { Message = "Article hidden." }) : NotFound(new { Message = "Article not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to hide article." + ex.Message });
            }
        }

        [HttpPost("{articleId}/unhide")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnhideArticle(int articleId)
        {
            try
            {
                var success = await _newsService.UnhideArticleAsync(articleId);
                return success ? Ok(new { Message = "Article unhidden." }) : NotFound(new { Message = "Article not found." });
            }
            catch (Exception ex)
            {
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
                var articles = await _newsService.GetPersonalizedArticlesAsync(userId);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to fetch personalized articles." + ex.Message });
            }
        }
    }
}
