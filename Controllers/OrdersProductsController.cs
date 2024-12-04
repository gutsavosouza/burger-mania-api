using burger_mania_api.Data;
using burger_mania_api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace burger_mania_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersProductsController : ControllerBase
    {
        private readonly DataContext _context;

        public OrdersProductsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/OrdersProducts/all
        [HttpGet("all")]
        public async Task<ActionResult<List<OrdersProducts>>> GetAllOrdersProducts(int page = 1, int pageSize = 10)
        {
            var ordersProducts = await _context.OrdersProducts
                .Include(op => op.Order)
                .Include(op => op.Product)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(ordersProducts);
        }

        // GET: api/OrdersProducts/get/{id}
        [HttpGet("get/{id}")]
        public async Task<ActionResult<OrdersProducts>> GetOrdersProductsById(int id)
        {
            var ordersProducts = await _context.OrdersProducts
                .Include(op => op.Order)
                .Include(op => op.Product)
                .FirstOrDefaultAsync(op => op.Id == id);

            if (ordersProducts is null)
                return NotFound(new { message = "OrdersProducts entry not found." });

            return Ok(ordersProducts);
        }

        // POST: api/OrdersProducts/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrdersProducts([FromBody] OrdersProducts ordersProducts)
        {
            // Validate OrderId
            var order = await _context.Orders.FindAsync(ordersProducts.OrderId);
            if (order == null)
                return BadRequest(new { message = "Invalid OrderId. Order does not exist." });

            // Validate ProductId
            var product = await _context.Products.FindAsync(ordersProducts.ProductId);
            if (product == null)
                return BadRequest(new { message = "Invalid ProductId. Product does not exist." });

            // Add new entry
            try
            {
                _context.OrdersProducts.Add(ordersProducts);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetOrdersProductsById), new { message = "Relation between product and order created with sucess" });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "An error occurred while creating the OrdersProducts entry." });
            }
        }

        // PUT: api/OrdersProducts/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateOrdersProducts(int id, [FromBody] OrdersProducts updatedOrdersProducts)
        {
            var existingOrdersProducts = await _context.OrdersProducts.FindAsync(id);
            if (existingOrdersProducts is null)
                return NotFound(new { message = "OrdersProducts entry not found." });

            // Validate OrderId
            var order = await _context.Orders.FindAsync(updatedOrdersProducts.OrderId);
            if (order == null)
                return BadRequest(new { message = "Invalid OrderId. Order does not exist." });

            // Validate ProductId
            var product = await _context.Products.FindAsync(updatedOrdersProducts.ProductId);
            if (product == null)
                return BadRequest(new { message = "Invalid ProductId. Product does not exist." });

            // Update properties
            existingOrdersProducts.OrderId = updatedOrdersProducts.OrderId;
            existingOrdersProducts.ProductId = updatedOrdersProducts.ProductId;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "OrdersProducts entry updated successfully." });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "An error occurred while updating the OrdersProducts entry." });
            }
        }

        // DELETE: api/OrdersProducts/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteOrdersProducts(int id)
        {
            var ordersProducts = await _context.OrdersProducts.FindAsync(id);
            if (ordersProducts is null)
                return NotFound(new { message = "OrdersProducts entry not found." });

            _context.OrdersProducts.Remove(ordersProducts);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "OrdersProducts entry deleted successfully." });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the OrdersProducts entry." });
            }
        }
    }
}
