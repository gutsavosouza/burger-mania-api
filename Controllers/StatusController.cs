using burger_mania_api.Data;
using burger_mania_api.DTOs;
using burger_mania_api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace burger_mania_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusesController : ControllerBase
    {
        private readonly DataContext _context;

        public StatusesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Status>>> GetAllStatuses(int page = 1, int pageSize = 10)
        {
            var statuses = await _context.Statuses
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(statuses);
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<Status>> GetStatusById(int id)
        {
            var status = await _context.Statuses
                .Include(s => s.Orders)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (status is null) return NotFound(new { message = "Status not found." });

            return Ok(status);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateStatus(StatusDTO statusDTO)
        {
            var status = new Status
            {
                Name = statusDTO.Name
            };

            try
            {
                _context.Statuses.Add(status);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetStatusById), new { id = status.Id }, new { message = "Status created." });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "An error occurred while creating the status." });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, StatusDTO statusDTO)
        {
            var status = await _context.Statuses.FindAsync(id);
            if (status is null)
                return NotFound(new { message = "Status not found." });

            status.Name = statusDTO.Name;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Status updated." });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteStatus(int id)
        {
            var status = await _context.Statuses.FindAsync(id);
            if (status is null)
                return NotFound(new { message = "Status not found." });

            _context.Statuses.Remove(status);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Status deleted." });
        }
    }
}
