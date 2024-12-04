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
                .Include(o => o.OrdersProducts)
                .ThenInclude(op => op.Product)
                .Include(o => o.Status)
                .Include(o => o.UsersOrders)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
            {
                var order = await _context.Orders
                .Include(o => o.Status)
                .Include(o => o.OrdersProducts)
                .Include(o => o.UsersOrders)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order is null) return NotFound(new { message = "Order not found." });

            return Ok(order);
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDTO request)
        {
            if (request.Products == null || !request.Products.Any())
            {
                return BadRequest(new { message = "Products cannot be null or empty." });
            }

            if (request.UserId == 0)
            {
                return BadRequest(new { message = "User ID cannot be null or zero." });
            }

            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid User ID. User does not exist." });
            }

            var productIds = request.Products.Select(p => p.ProductId).ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            if (products.Count != request.Products.Count)
            {
                return BadRequest(new { message = "One or more Product IDs are invalid." });
            }

            var order = new Order
            {
                StatusId = 1 // Definindo o padrão para PENDENTE(id = 1)
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var ordersProducts = request.Products.Select(product => new OrdersProducts
            {
                OrderId = order.Id,
                ProductId = product.ProductId,
                Quantity = product.Quantity
            }).ToList();

            _context.OrdersProducts.AddRange(ordersProducts);

            var usersOrders = new UsersOrders
            {
                UserId = request.UserId,
                OrderId = order.Id
            };

            _context.UsersOrders.Add(usersOrders);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Order created successfully." });
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
