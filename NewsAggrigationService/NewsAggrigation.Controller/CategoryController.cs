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

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            var result = await _service.CreateCategoryAsync(request);
            return CreatedAtAction(nameof(GetCategory), new { id = result.CategoryId }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var result = await _service.GetCategoryWithKeywordsAsync(id);
            return Ok(result);
        }

        [HttpPost("{id}/keywords")]
        public async Task<IActionResult> AddKeywords(int id, [FromBody] CreateKeywordRequest request)
        {
            await _service.AddKeywordsAsync(id, request);
            return Ok(new { Message = "Keywords added successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteCategory(int id)
        {
            await _service.SoftDeleteCategoryAsync(id);
            return NoContent();
        }

        [HttpDelete("keywords/{id}")]
        public async Task<IActionResult> SoftDeleteKeyword(int id)
        {
            await _service.SoftDeleteKeywordAsync(id);
            return NoContent();
        }
    }
}
