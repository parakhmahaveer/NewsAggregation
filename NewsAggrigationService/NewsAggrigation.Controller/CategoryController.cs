using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.BLL.Services.Catgory;
using NewsAggrigation.BLL.Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.Controller
{
    [ApiController]
    [Route("api/categories")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (ApiExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to retrieve categories." + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategoryAsync([FromQuery] string category)
        {
            try
            {
                var created = await _categoryService.CreateCategoryAsync(category);
                return CreatedAtAction(nameof(GetCategoryByIdAsync), new { id = created.CategoryId }, created);
            }
            catch (ApiExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to create category." + ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryByIdAsync(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryWithKeywordsAsync(id);
                return Ok(category);
            }
            catch (ApiExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to fetch category." + ex.Message });
            }
        }

        [HttpPost("{categoryId}/keywords")]
        public async Task<IActionResult> AddKeywordsToCategoryAsync(int categoryId, [FromBody] CreateKeywordRequest request)
        {
            try
            {
                await _categoryService.AddKeywordsAsync(categoryId, request);
                return Ok(new { Message = "Keywords added successfully." });
            }
            catch (ApiExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to add keywords." + ex.Message });
            }
        }

        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> DeleteCategoryAsync(int categoryId)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(categoryId);
                return NoContent();
            }
            catch (ApiExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to delete category." + ex.Message });
            }
        }

        [HttpDelete("keywords/{keywordId}")]
        public async Task<IActionResult> DeleteKeywordAsync(int keywordId)
        {
            try
            {
                await _categoryService.DeleteKeywordAsync(keywordId);
                return NoContent();
            }
            catch (ApiExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to delete keyword." + ex.Message });
            }
        }

        [HttpPost("{categoryId}/hide")]
        public async Task<IActionResult> HideCategoryAsync(int categoryId)
        {
            try
            {
                var success = await _categoryService.HideCategoryAsync(categoryId);
                return success ? Ok(new { Message = "Category hidden." }) : NotFound(new { Message = "Category not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to hide category." + ex.Message });
            }
        }

        [HttpPost("{categoryId}/unhide")]
        public async Task<IActionResult> UnhideCategoryAsync(int categoryId)
        {
            try
            {
                var success = await _categoryService.UnhideCategoryAsync(categoryId);
                return success ? Ok(new { Message = "Category unhidden." }) : NotFound(new { Message = "Category not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to unhide category." + ex.Message });
            }
        }

        [HttpPost("block")]
        public async Task<IActionResult> BlockArticlesByKeywordAsync([FromBody] string keyword)
        {
            try
            {
                var count = await _categoryService.BlockArticlesByKeywordAsync(keyword);
                return Ok(new { Message = $"{count} articles blocked for keyword '{keyword}'." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to block articles." + ex.Message });
            }
        }
    }
}
