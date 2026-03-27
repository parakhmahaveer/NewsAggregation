using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.BLL.Exceptions;
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
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetAllCategoriesAsync()
        {
            _logger.LogInformation("Getting all categories.");
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                _logger.LogInformation("Successfully retrieved categories.");
                return Ok(categories);
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogWarning(ex, "Unauthorized error while retrieving categories.");
                return Unauthorized();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Not found error while retrieving categories.");
                return NotFound(new { ex.Message });
            }
            catch (ApiExceptionHelper ex)
            {
                _logger.LogWarning(ex, "API exception occurred while retrieving categories.");
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve categories.");
                return StatusCode(500, new { Message = "Failed to retrieve categories." + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategoryAsync([FromQuery] string category)
        {
            _logger.LogInformation("Creating category: {Category}", category);
            try
            {
                var created = await _categoryService.CreateCategoryAsync(category);
                _logger.LogInformation("Category created with ID: {CategoryId}", created.CategoryId);
                return Ok();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error while creating category.");
                return BadRequest(new { ex.Message });
            }
            catch (ConflictException ex)
            {
                _logger.LogWarning(ex, "Conflict error while creating category.");
                return Conflict(new { ex.Message });
            }
            catch (ApiExceptionHelper ex)
            {
                _logger.LogWarning(ex, "API exception occurred while creating category.");
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create category.");
                return StatusCode(500, new { Message = "Failed to create category." + ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryByIdAsync(int id)
        {
            _logger.LogInformation("Getting category by ID: {CategoryId}", id);
            try
            {
                var category = await _categoryService.GetCategoryWithKeywordsAsync(id);
                _logger.LogInformation("Successfully retrieved category with ID: {CategoryId}", id);
                return Ok(category);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Not found error while fetching category by ID: {CategoryId}", id);
                return NotFound(new { ex.Message });
            }
            catch (ApiExceptionHelper ex)
            {
                _logger.LogWarning(ex, "API exception occurred while fetching category by ID: {CategoryId}", id);
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch category by ID: {CategoryId}", id);
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
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error while adding keyword category.");
                return BadRequest(new { ex.Message });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Not found error while fetching category");
                return NotFound(new { ex.Message });
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

        [HttpPost("{categoryId}/hide")]
        public async Task<IActionResult> HideCategoryAsync(int categoryId)
        {
            try
            {
                var success = await _categoryService.HideCategoryAsync(categoryId);
                return success ? Ok(new { Message = "Category hidden." }) : NotFound(new { Message = "Category not found." });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Not found error while fetching category");
                return NotFound(new { ex.Message });
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
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Not found error while fetching category");
                return NotFound(new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to unhide category." + ex.Message });
            }
        }

        [HttpPost("block")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BlockArticlesByKeywordAsync([FromQuery] string keyword)
        {
            try
            {
                var count = await _categoryService.BlockArticlesByKeywordAsync(keyword);
                return Ok(new { Message = $"{count} articles blocked for keyword '{keyword}'." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error while checking keyword.");
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to block articles." + ex.Message });
            }
        }
    }
}
