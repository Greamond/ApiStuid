using ApiStuid.DbWork;
using ApiStuid.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModelsTask = ApiStuid.Models.Task;
using ApiStuid.Models;

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
            var task = new ModelsTask
            {
                Name = request.Name,
                Description = request.Description,
                ProjectId = request.ProjectId,
                Chapter = 1
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
        }
    }
}
