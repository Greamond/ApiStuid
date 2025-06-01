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
        public async Task<ActionResult<Subtask>> PostSubtask(Subtask subtask)
        {
            _context.Subtasks.Add(subtask);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSubtask), new { id = subtask.Id }, subtask);
        }

        // PUT: api/Subtasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubtask(int id, Subtask subtask)
        {
            if (id != subtask.Id)
            {
                return BadRequest();
            }

            _context.Entry(subtask).State = EntityState.Modified;

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

            return NoContent();
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

        private bool SubtaskExists(int id)
        {
            return _context.Subtasks.Any(e => e.Id == id);
        }
    }
}
