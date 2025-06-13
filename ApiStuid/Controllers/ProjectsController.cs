using ApiStuid.DbWork;
using ApiStuid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiStuid.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public ProjectsController(DatabaseContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost("createProject")]
        public async Task<ActionResult<Project>> CreateProject([FromBody] ProjectCreateRequest request)
        {
            // Получаем ID текущего пользователя из токена
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var project = new Project
            {
                Name = request.Name,
                Description = request.Description,
                IsPublic = request.IsPublic,
                Creator = userId // Используем ID из токена
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            _context.ChaptersTask.Add(new ChapterTask
            {
                Name = "Новые",
                ProjectId = project.Id
            });
            _context.ChaptersTask.Add(new ChapterTask
            {
                Name = "В работе",
                ProjectId = project.Id
            });
            _context.ChaptersTask.Add(new ChapterTask
            {
                Name = "Выполненные",
                ProjectId = project.Id
            });
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }

        public class ProjectCreateRequest
        {
            public string Name { get; set; }

            public string Description { get; set; }

            public bool IsPublic { get; set; }
        }

        // GET: api/Projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            return await _context.Projects.ToListAsync();
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, [FromBody] ProjectUpdateRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            // Получаем текущего пользователя из токена
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            // Проверяем, что текущий пользователь является создателем проекта
            if (project.Creator != currentUserId)
            {
                return Forbid("Only project creator can update project");
            }

            project.Name = request.Name;
            project.Description = request.Description;
            project.IsPublic = request.IsPublic;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            // Получаем текущего пользователя из токена
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            // Проверяем, что текущий пользователь является создателем проекта
            if (project.Creator != currentUserId)
            {
                return Forbid("Only project creator can delete project");
            }

            // Удаляем всех участников проекта
            var participants = await _context.Participants
                .Where(p => p.ProjectId == id)
                .ToListAsync();
            _context.Participants.RemoveRange(participants);

            // Удаляем сам проект
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public class ProjectUpdateRequest
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public bool IsPublic { get; set; }
        }

        // GET: api/Projects/forUser/5
        [HttpGet("forUser/{userId}")]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjectsForUser(int userId)
        {
            // Получаем все публичные проекты
            var publicProjects = await _context.Projects
                .Where(p => p.IsPublic)
                .ToListAsync();

            // Получаем проекты, где пользователь является создателем
            var userCreatedProjects = await _context.Projects
                .Where(p => p.Creator == userId)
                .ToListAsync();

            // Получаем проекты, где пользователь является участником
            var userParticipantProjects = await _context.Projects
                .Join(_context.Participants,
                    p => p.Id,
                    part => part.ProjectId,
                    (p, part) => new { Project = p, Participant = part })
                .Where(x => x.Participant.UserId == userId)
                .Select(x => x.Project)
                .ToListAsync();

            // Объединяем и устраняем дубликаты
            var allProjects = publicProjects
                .Union(userCreatedProjects)
                .Union(userParticipantProjects)
                .Distinct()
                .ToList();

            return allProjects;
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
