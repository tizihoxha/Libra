using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Libra.DATA;
using Libra.Models;
using Libra.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Libra.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private ApiResponse _response;
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
            _response = new ApiResponse();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _context.Category.ToListAsync();
        }

        [HttpGet("{id:int}", Name = "GetCategory")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Category.FindAsync(id);

            if (category == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }

            return category;
        }

        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory([FromForm] CategoryCreateDTO categoryCreateDTO)
        {
            var category = new Category
            {
                Name = categoryCreateDTO.Name,
                Priority = categoryCreateDTO.Priority,
                CreatedAt = DateTime.Now.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'"),
                CreatedBy = categoryCreateDTO.CreatedBy
        };

            _context.Category.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromForm] CategoryCreateDTO categoryCreateDTO)
        {
            var category = await _context.Category.FindAsync(id);

            if (category == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }

            category.Name = categoryCreateDTO.Name;
            category.Priority = categoryCreateDTO.Priority;
            category.CreatedAt = DateTime.Now.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'");
            category.CreatedBy = categoryCreateDTO.CreatedBy;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!CategoryExists(id))
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Category.FindAsync(id);

            if (category == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }

            _context.Category.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Category.Any(e => e.Id == id);
        }
    }
}
