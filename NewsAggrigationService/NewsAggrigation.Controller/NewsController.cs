using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.BLL.Services.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.Controller
{
    [ApiController]
    [Route("api/news")]
    [Authorize]
    public class NewsController: ControllerBase
    {
        private readonly INewsService _newsService;
        public NewsController (INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet("today")]
        public async Task<IActionResult> GetTodaysNews()
        {
            try
            {
                var articles = await _newsService.GetTodaysNewsAsync();
                return Ok(articles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("category")]
        public async Task<IActionResult> GetTodaysNewsByCategory(NewsByCategoryRequest request)
        {
            try
            {
                var articles = await _newsService.GetTodaysNewsByCategoryAsync(request);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("react")]
        public async Task<IActionResult> SetArticleReaction([FromBody] ArticleReactionRequest request)
        {
            try
            {
                var result = await _newsService.SetArticleReactionAsync(request);
                if (!result)
                    return BadRequest("Could not update feedback.");
                return Ok(request.IsLiked ? "Article liked." : "Article disliked.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("unsave")]
        public async Task<IActionResult> UnsaveArticle([FromQuery] string username, [FromQuery] int articleId)
        {
            var result = await _newsService.UnsaveArticleAsync(username, articleId);
            if (!result) return NotFound("Saved article not found.");
            return NoContent();
        }

        [HttpGet("saved/{username}")]
        public async Task<IActionResult> GetSavedArticles(string username)
        {
            var articles = await _newsService.GetSavedArticlesAsync(username);
            return Ok(articles);
        }
    }
}
