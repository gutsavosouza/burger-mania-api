using burger_mania_api.Data;
using burger_mania_api.DTOs;
using burger_mania_api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace burger_mania_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DataContext _context;

        public ProductController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Product>>> GetAllProducts(int page = 1, int pageSize = 10)
        {
            var products = await _context.Products
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            return Ok(products);
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if(product is null) return NotFound(new { message = "Product not found."});

            return Ok(product);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct(ProductDTO productDTO)
        {
            // Check if the CategoryId is provided and valid
            if (productDTO.CategoryId is not null)
            {
                var category = await _context.Categories.FindAsync(productDTO.CategoryId);
                if (category == null)
                {
                    return BadRequest(new { Message = "Invalid CategoryId. Category does not exist." });
                }
            }

            // Create a new Product object
            var product = new Product
            {
                Name = productDTO.Name,
                ImagePath = productDTO.ImagePath,
                Price = productDTO.Price,
                Ingredients = productDTO.Ingredients,
                Description = productDTO.Description,
                CategoryId = productDTO.CategoryId // This can be null
            };

            try
            {
                // Add the product to the database
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Return a response with the product's ID
                return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, new { message = "Product created" });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "An error occurred while creating the product." });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateProduct(ProductDTO productDTO, int id)
        {
            var product = await _context.Products.FindAsync(id);
            if(product is null) return NotFound(new { message = "Product not found."});

            product.Name = productDTO.Name;
            product.Price = productDTO.Price;
            product.Ingredients = productDTO.Ingredients;
            product.Description = productDTO.Description;
            product.CategoryId = productDTO.CategoryId;
            product.ImagePath = productDTO.ImagePath;
            
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product updated"});
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if(product is null) return NotFound(new { message = "Product not found."});

            _context.Remove(product);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Product deleted"});
        }
    }
}