using ApiStuid.DbWork;
using ApiStuid.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiStuid.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskResponsiblesController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public TaskResponsiblesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/TaskResponsibles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskResponsible>>> GetTaskResponsibles()
        {
            return await _context.TaskResponsibles.Include(tr => tr.User)
                                                 .Include(tr => tr.Task)
                                                 .ToListAsync();
        }

        // GET: api/TaskResponsibles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskResponsible>> GetTaskResponsible(int id)
        {
            var taskResponsible = await _context.TaskResponsibles.Include(tr => tr.User)
                                                                .Include(tr => tr.Task)
                                                                .FirstOrDefaultAsync(tr => tr.Id == id);

            if (taskResponsible == null)
            {
                return NotFound();
            }

            return taskResponsible;
        }

        // GET: api/TaskResponsibles/task/5
        [HttpGet("task/{taskId}")]
        public async Task<ActionResult<IEnumerable<TaskResponsible>>> GetResponsiblesByTask(int taskId)
        {
            return await _context.TaskResponsibles.Where(tr => tr.TaskId == taskId)
                                                 .Include(tr => tr.User)
                                                 .ToListAsync();
        }

        // POST: api/TaskResponsibles
        [HttpPost]
        public async Task<ActionResult<TaskResponsible>> PostTaskResponsible(TaskResponsible taskResponsible)
        {
            _context.TaskResponsibles.Add(taskResponsible);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaskResponsible), new { id = taskResponsible.Id }, taskResponsible);
        }

        // DELETE: api/TaskResponsibles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskResponsible(int id)
        {
            var taskResponsible = await _context.TaskResponsibles.FindAsync(id);
            if (taskResponsible == null)
            {
                return NotFound();
            }

            _context.TaskResponsibles.Remove(taskResponsible);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskResponsibleExists(int id)
        {
            return _context.TaskResponsibles.Any(e => e.Id == id);
        }
    }
}
