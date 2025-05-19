using ApiStuid.Classes;
using ApiStuid.DbWork;
using ApiStuid.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApiStuid.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly JwtService _jwtService;

        public AuthController(DatabaseContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken ct = default)
        {
            var emailParam = new MySqlParameter("@p_email", request.Email);
            var passwordParam = new MySqlParameter("@p_password", request.Password);

            var sql = "CALL VerifyUserPassword(@p_email, @p_password)";

            var results = await _context.LoginResults
                .FromSqlRaw(sql, emailParam, passwordParam)
                .ToListAsync(ct); // Загружаем всё в список

            var result = results.FirstOrDefault(); // Обрабатываем в памяти

            if (result == null || !result.IsValid || result.UserId == null)
                return Unauthorized("Invalid credentials");

            var userId = result.UserId.Value;

            var user = await _context.Users.FindAsync(new object[] { userId }, ct);

            if (user == null)
                return Unauthorized("User not found");

            // Обновление времени последней активности
            user.LastActivity = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);

            // Генерация токена
            var token = _jwtService.GenerateToken(user);

            return Ok(new AuthResponse
            {
                Token = token,
                EmployeeId = user.Id,
                LastName = user.LastName,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                Email = user.Email,
                Description = user.Description
            });
        }

        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class LoginResult
        {
            public bool IsValid { get; set; }
            public int? UserId { get; set; }
        }

        public class AuthResponse
        {
            public string Token { get; set; }
            public int EmployeeId { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string Email { get; set; }
            public string Description { get; set; }
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Email already exists");
            }

            // Create new user
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                Email = request.Email,
                Password = request.Password
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user);

            // Return similar response to login
            return Ok(new AuthResponse
            {
                Token = token,
                EmployeeId = user.Id,
                LastName = user.LastName,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                Email = user.Email,
                Description = user.Description
            });
        }

        public class RegisterRequest
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string MiddleName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}
