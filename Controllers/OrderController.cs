using burger_mania_api.Data;
using burger_mania_api.DTOs;
using burger_mania_api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace burger_mania_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly DataContext _context;

        public OrdersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Order>>> GetAllOrders(int page = 1, int pageSize = 10)
        {
            var orders = await _context.Orders
                .Include(o => o.Status) // Include related Status
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Status) // Include related Status
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order is null) return NotFound(new { message = "Order not found." });

            return Ok(order);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder(OrderDTO orderDTO)
        {
            // Validate StatusId
            var status = await _context.Statuses.FindAsync(orderDTO.StatusId);
            if (status == null)
                return BadRequest(new { message = "Invalid StatusId. Status does not exist." });

            var order = new Order
            {
                StatusId = orderDTO.StatusId,
                TotalPrice = orderDTO.TotalPrice
            };

            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, new { message = "Order created." });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "An error occurred while creating the order." });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateOrder(int id, OrderDTO orderDTO)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order is null)
                return NotFound(new { message = "Order not found." });

            // Validate StatusId
            var status = await _context.Statuses.FindAsync(orderDTO.StatusId);
            if (status == null)
                return BadRequest(new { message = "Invalid StatusId. Status does not exist." });

            order.StatusId = orderDTO.StatusId;
            order.TotalPrice = orderDTO.TotalPrice;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Order updated." });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order is null)
                return NotFound(new { message = "Order not found." });

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Order deleted." });
        }
    }
}
