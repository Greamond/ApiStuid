using ApiStuid.Classes;
using ApiStuid.DbWork;
using ApiStuid.Models;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
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
                var creator = await _context.Users.FindAsync(currentUserId);
                var creatorName = $"{creator.LastName} {creator.FirstName}";

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

                        // Отправляем уведомление, если есть FCM токен
                        if (!string.IsNullOrEmpty(user.FCMToken))
                        {
                            var notificationData = new Dictionary<string, string>
                            {
                                { "type", "project_invite" },
                                { "projectId", project.Id.ToString() },
                                { "projectName", project.Name },
                                { "inviterId", creator.Id.ToString() },
                                { "inviterName", creatorName }
                            };

                            await NotificationMobile.SendPushNotificationInvate(
                                fcmToken: user.FCMToken,
                                data: notificationData
                            );
                        }
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

        [HttpGet("getParticipants/{projectId}")]
        public async Task<IActionResult> GetParticipants(int projectId)
        {
            try
            {
                // Проверяем существование проекта
                var projectExists = await _context.Projects.AnyAsync(p => p.Id == projectId);
                if (!projectExists)
                {
                    return NotFound("Project not found");
                }

                // Получаем список участников с информацией о пользователях
                var participants = await _context.Participants
                    .Where(p => p.ProjectId == projectId)
                    .Include(p => p.User) // Включаем связанные данные пользователя
                    .Select(p => new
                    {
                        UserId = p.UserId,
                        FirstName = p.User.FirstName,
                        LastName = p.User.LastName,
                        MiddleName = p.User.MiddleName,
                        Email = p.User.Email
                        // Добавьте другие необходимые поля пользователя
                    })
                    .ToListAsync();

                return Ok(participants);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("updateParticipants")]
        public async Task<IActionResult> UpdateParticipants([FromBody] UpdateParticipantsRequest request)
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
                    return Forbid("Only project creator can update participants");
                }

                // Получаем текущих участников
                var currentParticipants = await _context.Participants
                    .Where(p => p.ProjectId == request.ProjectId)
                    .ToListAsync();

                // Удаляем участников, которых нет в новом списке
                var participantsToRemove = currentParticipants
                    .Where(cp => !request.ParticipantIds.Contains(cp.UserId))
                    .ToList();

                _context.Participants.RemoveRange(participantsToRemove);

                // Добавляем новых участников
                foreach (var participantId in request.ParticipantIds)
                {
                    // Проверяем, что пользователь существует
                    var user = await _context.Users.FindAsync(participantId);
                    if (user == null)
                    {
                        continue; // Пропускаем несуществующих пользователей
                    }

                    // Проверяем, что участник еще не добавлен
                    if (!currentParticipants.Any(cp => cp.UserId == participantId))
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

        public class UpdateParticipantsRequest
        {
            public int ProjectId { get; set; }
            public List<int> ParticipantIds { get; set; }
        }
    }
}
