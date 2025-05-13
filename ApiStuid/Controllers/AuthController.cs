using ApiStuid.DbWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ApiStuid.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public AuthController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);

            if (user == null)
                return Unauthorized("Invalid credentials");

            // Здесь можно добавить генерацию JWT токена, если нужно
            return Ok(new AuthResponse
            {
                EmployeeId = user.Id,
                LastName = user.LastName,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                Email = user.Email,
                Description = user.Description
            });
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AuthResponse
    {
        public int EmployeeId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
    }
}
