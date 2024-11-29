using burger_mania_api.Data;
using burger_mania_api.DTOs;
using burger_mania_api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace burger_mania_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;

        public UserController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<User>>> GetAllUsers(int page = 1, int pageSize = 10)
        {
            var users = await _context.Users
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            return Ok(users);
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if(user is null)
            {
                return NotFound(new { message = "User not found."});
            }
            return Ok(user);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(UserDTO userDto)
        {
            var user = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password)
            };

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, new { message = "User created" });
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("duplicate key") == true) // SQL Server/PostgreSQL
                {
                    return Conflict(new { message = "Email already exists." });
                }

                return StatusCode(500, new { message = "An error occurred while creating the user." });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(UserDTO userDto, int id)
        {
            var user = await _context.Users.FindAsync(id);
            if(user is null)
            {
                return NotFound(new { message = "User not found."});
            }

            user.Name = userDto.Name;
            user.Email = userDto.Email;
            user.Password = userDto.Password;

            await _context.SaveChangesAsync();

            return Ok(new { message = "User updated"});
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if(user is null) return NotFound(new { message = "User not found."});

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted"});
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            // Find the user by email
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user is null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            // Verify the password
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password);
            if (!isPasswordValid)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            return Ok(new { message = "Login successful" });
        }
    }
}