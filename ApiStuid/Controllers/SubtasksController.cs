using ApiStuid.DbWork;
using ApiStuid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiStuid.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SubtasksController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public SubtasksController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Subtasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subtask>>> GetSubtasks()
        {
            return await _context.Subtasks.ToListAsync();
        }

        // GET: api/Subtasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Subtask>> GetSubtask(int id)
        {
            var subtask = await _context.Subtasks.FindAsync(id);

            if (subtask == null)
            {
                return NotFound();
            }

            return subtask;
        }

        // GET: api/Subtasks/task/5
        [HttpGet("task/{taskId}")]
        public async Task<ActionResult<IEnumerable<Subtask>>> GetSubtasksByTask(int taskId)
        {
            return await _context.Subtasks.Where(s => s.TaskId == taskId).ToListAsync();
        }

        // POST: api/Subtasks
        [HttpPost]
        public async Task<ActionResult<Subtask>> PostSubtask([FromBody] SubtaskCreateRequest request)
        {
            // Проверяем существование пользователя
            var creatorExists = await _context.Users.AnyAsync(u => u.Id == request.CreatorId);
            if (!creatorExists)
            {
                return BadRequest("Указанный создатель не существует");
            }

            // Проверяем существование проекта
            var taskExists = await _context.Tasks.AnyAsync(p => p.Id == request.TaskId);
            if (!taskExists)
            {
                return BadRequest("Указанная задача не существует");
            }

            var subtask = new Subtask
            {
                Name = request.Name,
                Description = request.Description,
                TaskId = request.TaskId,
                Responsible = request.Responsible,
                ChapterId = request.ChapterId,
                CreatorId = request.CreatorId,
                Position = request.Position
            };

            _context.Subtasks.Add(subtask);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSubtask), new { id = subtask.Id }, subtask);
        }

        public class SubtaskCreateRequest
        {
            public string Name { get; set; }
            public int TaskId { get; set; }
            public string Description { get; set; }
            public int ChapterId { get; set; }
            public int Responsible { get; set; }
            public int CreatorId { get; set; }
            public int Position { get; set; }
        }

        // PUT: api/Subtasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] SubtaskUpdateRequest request)
        {
            var subtask = await _context.Subtasks.FindAsync(id);
            if (subtask == null)
            {
                return NotFound();
            }

            // Обновляем основные поля
            subtask.Name = request.Name ?? subtask.Name;
            subtask.Description = request.Description ?? subtask.Description;
            subtask.ChapterId = request.ChapterId;
            subtask.Responsible = request.Responsible;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubtaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(subtask);
        }

        public class SubtaskUpdateRequest
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public int ChapterId { get; set; }
            public int Responsible { get; set; }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateSubtaskStatus(int id, [FromBody] SubtaskStatusUpdateRequest request)
        {
            if (request == null || request.ChapterId <= 0)
            {
                return BadRequest("Invalid ChapterId");
            }

            var subtask = await _context.Subtasks.FindAsync(id);
            if (subtask == null)
            {
                return NotFound();
            }

            subtask.ChapterId = request.ChapterId;
            await _context.SaveChangesAsync();

            return Ok();
        }

        public class SubtaskStatusUpdateRequest
        {
            public int ChapterId { get; set; }
        }

        // DELETE: api/Subtasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubtask(int id)
        {
            var subtask = await _context.Subtasks.FindAsync(id);
            if (subtask == null)
            {
                return NotFound();
            }

            _context.Subtasks.Remove(subtask);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("update-order")]
        public async Task<IActionResult> UpdateTaskOrder([FromBody] UpdateSubtaskOrderRequest request)
        {
            if (request == null || request.SubtaskOrder == null || !request.SubtaskOrder.Any())
            {
                return BadRequest("Invalid request data");
            }

            var subtaskIds = request.SubtaskOrder.Select(t => t.SubtaskId).ToList();
            var subtasks = await _context.Subtasks
                .Where(t => subtaskIds.Contains(t.Id) && t.TaskId == request.TaskId)
                .ToListAsync();

            if (subtasks.Count != subtaskIds.Count)
            {
                return BadRequest("Some tasks do not belong to the specified project");
            }

            foreach (var item in request.SubtaskOrder)
            {
                var subtask = subtasks.FirstOrDefault(t => t.Id == item.SubtaskId);
                if (subtask != null)
                {
                    subtask.Position = item.Position;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public class UpdateSubtaskOrderRequest
        {
            public int TaskId { get; set; }
            public int ColumnId { get; set; }
            public List<SubtaskOrderItem> SubtaskOrder { get; set; }
        }

        public class SubtaskOrderItem
        {
            public int SubtaskId { get; set; }
            public int Position { get; set; }
        }

        private bool SubtaskExists(int id)
        {
            return _context.Subtasks.Any(e => e.Id == id);
        }
    }
}
