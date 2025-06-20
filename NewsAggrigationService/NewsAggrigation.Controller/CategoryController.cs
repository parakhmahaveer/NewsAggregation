using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.BLL.Services.Catgory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.Controller
{
    [ApiController]
    [Route("api/categories")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var result = await _service.GetAllCategoriesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromQuery] string category)
        {
            try
            {
                var result = await _service.CreateCategoryAsync(category);
                return CreatedAtAction(nameof(GetCategory), new { id = result.CategoryId }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            try
            {
                var result = await _service.GetCategoryWithKeywordsAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{id}/keywords")]
        public async Task<IActionResult> AddKeywords(int id, [FromBody] CreateKeywordRequest request)
        {
            try
            {
                await _service.AddKeywordsAsync(id, request);
                return Ok(new { Message = "Keywords added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _service.DeleteCategoryAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("keywords/{id}")]
        public async Task<IActionResult> DeleteKeyword(int id)
        {
            try
            {
                await _service.DeleteKeywordAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
