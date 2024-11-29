using burger_mania_api.Data;
using burger_mania_api.DTOs;
using burger_mania_api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace burger_mania_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly DataContext _context;

        public CategoryController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Category>>> GetAllCategories(int page = 1, int pageSize = 10)
        {
            var categories = await _context.Categories
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(categories);
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<Category>> GetCategoryById(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category is null)
                return NotFound(new { message = "Category not found." });

            return Ok(category);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCategory(CategoryDTO categoryDTO)
        {
            if (await _context.Categories.AnyAsync(c => c.Name == categoryDTO.Name))
                return BadRequest(new { message = "Category with this name already exists." });

            var category = new Category
            {
                Name = categoryDTO.Name,
                Description = categoryDTO.Description,
                ImagePath = categoryDTO.ImagePath
            };

            try
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, new { message = "Category created" });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "An error occurred while creating the category." });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCategory(CategoryDTO categoryDTO, int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category is null)
                return NotFound(new { message = "Category not found." });

            category.Name = categoryDTO.Name;
            category.Description = categoryDTO.Description;
            category.ImagePath = categoryDTO.ImagePath;

            if (await _context.Categories.AnyAsync(c => c.Name == categoryDTO.Name && c.Id != id))
                return BadRequest(new { message = "Another category with this name already exists." });

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Category updated" });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "An error occurred while updating the category." });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category is null)
                return NotFound(new { message = "Category not found." });

            var products = await _context.Products.Where(p => p.CategoryId == id).ToListAsync();
            foreach (var product in products)
            {
                product.CategoryId = null;
            }

            _context.Categories.Remove(category);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Category deleted" });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the category." });
            }
        }
    }
}
