using burger_mania_api.Data;
using burger_mania_api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace burger_mania_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersOrdersController : ControllerBase
    {
        private readonly DataContext _context;

        public UsersOrdersController(DataContext context)
        {
            _context = context;
        }

        // GET: api/UsersOrders/all
        [HttpGet("all")]
        public async Task<ActionResult<List<UsersOrders>>> GetAllUsersOrders(int page = 1, int pageSize = 10)
        {
            var usersOrders = await _context.UsersOrders
                .Include(uo => uo.User)
                .Include(uo => uo.Order)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(usersOrders);
        }

        // GET: api/UsersOrders/get/{id}
        [HttpGet("get/{id}")]
        public async Task<ActionResult<UsersOrders>> GetUsersOrdersById(int id)
        {
            var usersOrder = await _context.UsersOrders
                .Include(uo => uo.User)
                .Include(uo => uo.Order)
                .FirstOrDefaultAsync(uo => uo.Id == id);

            if (usersOrder is null)
                return NotFound(new { message = "UsersOrders entry not found." });

            return Ok(usersOrder);
        }

        // POST: api/UsersOrders/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateUsersOrders([FromBody] UsersOrders usersOrders)
        {
            // Validate UserId
            var user = await _context.Users.FindAsync(usersOrders.UserId);
            if (user == null)
                return BadRequest(new { message = "Invalid UserId. User does not exist." });

            // Validate OrderId
            var order = await _context.Orders.FindAsync(usersOrders.OrderId);
            if (order == null)
                return BadRequest(new { message = "Invalid OrderId. Order does not exist." });

            // Add new entry
            try
            {
                _context.UsersOrders.Add(usersOrders);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetUsersOrdersById), new { id = usersOrders.Id }, new { message = "Relation between user and order created successfully." });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "An error occurred while creating the UsersOrders entry." });
            }
        }

        // PUT: api/UsersOrders/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUsersOrders(int id, [FromBody] UsersOrders updatedUsersOrders)
        {
            var existingUsersOrders = await _context.UsersOrders.FindAsync(id);
            if (existingUsersOrders is null)
                return NotFound(new { message = "UsersOrders entry not found." });

            // Validate UserId
            var user = await _context.Users.FindAsync(updatedUsersOrders.UserId);
            if (user == null)
                return BadRequest(new { message = "Invalid UserId. User does not exist." });

            // Validate OrderId
            var order = await _context.Orders.FindAsync(updatedUsersOrders.OrderId);
            if (order == null)
                return BadRequest(new { message = "Invalid OrderId. Order does not exist." });

            // Update properties
            existingUsersOrders.UserId = updatedUsersOrders.UserId;
            existingUsersOrders.OrderId = updatedUsersOrders.OrderId;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "UsersOrders entry updated successfully." });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "An error occurred while updating the UsersOrders entry." });
            }
        }

        // DELETE: api/UsersOrders/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUsersOrders(int id)
        {
            var usersOrders = await _context.UsersOrders.FindAsync(id);
            if (usersOrders is null)
                return NotFound(new { message = "UsersOrders entry not found." });

            _context.UsersOrders.Remove(usersOrders);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "UsersOrders entry deleted successfully." });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the UsersOrders entry." });
            }
        }
    }
}
