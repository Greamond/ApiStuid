using ApiStuid.DbWork;
using ApiStuid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiStuid.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public UsersController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet("employees")]
        public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetEmployees()
        {
            return await _context.Users
                .Select(u => new EmployeeResponse
                {
                    EmployeeId = u.Id,
                    LastName = u.LastName,
                    FirstName = u.FirstName,
                    MiddleName = u.MiddleName,
                    Email = u.Email,
                    Description = u.Description
                })
                .ToListAsync();
        }

        public class EmployeeResponse
        {
            public int EmployeeId { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string Email { get; set; }
            public string Description { get; set; }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateRequest request)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "User not found" });
                }

                // Обновление полей
                user.LastName = request.LastName;
                user.FirstName = request.FirstName;
                user.MiddleName = request.MiddleName;
                user.Description = request.Description;

                await _context.SaveChangesAsync();

                // Явно возвращаем JSON с успешным статусом
                return Ok(new
                {
                    success = true,
                    message = "Profile updated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Update failed",
                    error = ex.Message
                });
            }
        }

        public class UserUpdateRequest
        {
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string Description { get; set; }
        }

        [HttpPut("{id}/photo")]
        public async Task<IActionResult> UpdateUserPhoto(int id, [FromBody] UserPhotoUpdateRequest request)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "User not found" });
                }

                // Конвертируем Base64 в byte[]
                if (!string.IsNullOrEmpty(request.Photo))
                {
                    user.Photo = Convert.FromBase64String(request.Photo);
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Photo updated successfully"
                });
            }
            catch (FormatException)
            {
                return BadRequest(new { success = false, message = "Invalid photo format" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        public class UserPhotoUpdateRequest
        {
            public string Photo { get; set; }
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpGet("{id}/photo")]
        public async Task<IActionResult> GetUserPhoto(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || user.Photo == null)
            {
                return NotFound(new { success = false, message = "Photo not found" });
            }

            return Ok(new
            {
                success = true,
                photo = Convert.ToBase64String(user.Photo)
            });
        }
    }
}
