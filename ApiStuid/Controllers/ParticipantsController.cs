using ApiStuid.DbWork;
using ApiStuid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiStuid.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipantsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public ParticipantsController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpPost("addParticipants")]
        public async Task<IActionResult> AddParticipants([FromBody] AddParticipantsRequest request)
        {
            try
            {
                // Проверяем существование проекта
                var project = await _context.Projects.FindAsync(request.ProjectId);
                if (project == null)
                {
                    return NotFound("Project not found");
                }

                // Получаем текущего пользователя из токена
                var currentUserId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

                // Проверяем, что текущий пользователь является создателем проекта
                if (project.Creator != currentUserId)
                {
                    return Forbid("Only project creator can add participants");
                }

                // Добавляем участников
                foreach (var participantId in request.ParticipantIds)
                {
                    // Проверяем, что пользователь существует
                    var user = await _context.Users.FindAsync(participantId);
                    if (user == null)
                    {
                        continue; // Пропускаем несуществующих пользователей
                    }

                    // Проверяем, что участник еще не добавлен
                    var existingParticipant = await _context.Participants
                        .FirstOrDefaultAsync(p => p.ProjectId == request.ProjectId && p.UserId == participantId);

                    if (existingParticipant == null)
                    {
                        _context.Participants.Add(new Participant
                        {
                            ProjectId = request.ProjectId,
                            UserId = participantId
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public class AddParticipantsRequest
        {
            public int ProjectId { get; set; }
            public List<int> ParticipantIds { get; set; }
        }
    }
}
