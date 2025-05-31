using ApiStuid.DbWork;
using ApiStuid.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModelsTask = ApiStuid.Models.Task;
using ApiStuid.Models;
using System.ComponentModel.DataAnnotations;

namespace ApiStuid.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public TasksController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ModelsTask>> GetTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            // Загружаем ответственных за задачу
            var responsibles = await _context.TaskResponsibles
                .Where(tr => tr.TaskId == id)
                .ToListAsync();

            return task;
        }

        // GET: api/Tasks/project/5
        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<IEnumerable<ModelsTask>>> GetTasksByProject(int projectId)
        {
            return await _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<ModelsTask>> PostTask([FromBody] TaskCreateRequest request)
        {
            // Проверяем существование пользователя
            var creatorExists = await _context.Users.AnyAsync(u => u.Id == request.CreatorId);
            if (!creatorExists)
            {
                return BadRequest("Указанный создатель не существует");
            }

            // Проверяем существование проекта
            var projectExists = await _context.Projects.AnyAsync(p => p.Id == request.ProjectId);
            if (!projectExists)
            {
                return BadRequest("Указанный проект не существует");
            }

            var task = new ModelsTask
            {
                Name = request.Name,
                Description = request.Description,
                ProjectId = request.ProjectId,
                Chapter = request.ChapterId,
                CreatorId = request.CreatorId,
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Добавляем ответственных
            if (request.AssigneeIds != null)
            {
                foreach (var assigneeId in request.AssigneeIds)
                {
                    _context.TaskResponsibles.Add(new TaskResponsible
                    {
                        TaskId = task.Id,
                        UserId = assigneeId
                    });
                }
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        public class TaskCreateRequest
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public int ProjectId { get; set; }
            public int ChapterId { get; set; }
            public List<int> AssigneeIds { get; set; }
            public int CreatorId { get; set; }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskUpdateRequest request)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            // Обновляем основные поля
            task.Name = request.Name ?? task.Name;
            task.Description = request.Description ?? task.Description;
            task.Chapter = request.ChapterId;

            // Обновляем ответственных
            if (request.AssigneeIds != null)
            {
                // Удаляем старых ответственных
                var existingResponsibles = await _context.TaskResponsibles
                    .Where(tr => tr.TaskId == id)
                    .ToListAsync();

                _context.TaskResponsibles.RemoveRange(existingResponsibles);

                // Добавляем новых
                foreach (var assigneeId in request.AssigneeIds)
                {
                    _context.TaskResponsibles.Add(new TaskResponsible
                    {
                        TaskId = id,
                        UserId = assigneeId
                    });
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(task);
        }

        public class TaskUpdateRequest
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public int ChapterId { get; set; }
            public List<int>? AssigneeIds { get; set; }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] TaskStatusUpdateRequest request)
        {
            if (request == null || request.ChapterId <= 0)
            {
                return BadRequest("Invalid ChapterId");
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            task.Chapter = request.ChapterId;
            await _context.SaveChangesAsync();

            return Ok();
        }

        public class TaskStatusUpdateRequest
        {
            public int ChapterId { get; set; }
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
