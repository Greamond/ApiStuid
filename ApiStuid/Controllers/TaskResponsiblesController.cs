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
    public class TaskResponsiblesController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public TaskResponsiblesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/TaskResponsibles/task/5 - Получить всех ответственных для задачи
        [HttpGet("task/{taskId}")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAssigneesForTask(int taskId)
        {
            var assignees = await _context.TaskResponsibles
                .Where(tr => tr.TaskId == taskId)
                .Join(_context.Users,
                    tr => tr.UserId,
                    u => u.Id,
                    (tr, u) => new Employee
                    {
                        EmployeeId = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        MiddleName = u.MiddleName,
                    })
                .ToListAsync();

            if (!assignees.Any())
            {
                return NotFound();
            }

            return assignees;
        }

        public class Employee
        {
            public int EmployeeId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string MiddleName { get; set; }
        }

        // GET: api/TaskResponsibles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskResponsible>>> GetTaskResponsibles()
        {
            return await _context.TaskResponsibles.ToListAsync();
        }

        // GET: api/TaskResponsibles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskResponsible>> GetTaskResponsible(int id)
        {
            var taskResponsible = await _context.TaskResponsibles.FirstOrDefaultAsync(x => x.Id == id);

            if (taskResponsible == null)
            {
                return NotFound();
            }

            return taskResponsible;
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
