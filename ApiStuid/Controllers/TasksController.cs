﻿using ApiStuid.DbWork;
using ApiStuid.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModelsTask = ApiStuid.Models.Task;
using ApiStuid.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using ApiStuid.Classes;
using System;
using System.Security.Claims;

namespace ApiStuid.Controllers
{
    [Authorize]   
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

        [HttpGet("assignee/{assigneeId}")]
        public async Task<ActionResult<IEnumerable<ModelsTask>>> GetTasksByAssignee(int assigneeId)
        {
            var responsibles = await _context.TaskResponsibles.Where(r => r.UserId == assigneeId).Select(r => r.TaskId).ToListAsync();
            var tasks = await _context.Tasks.Where(t => responsibles.Contains(t.Id) || t.CreatorId == assigneeId).ToListAsync();

            if (!tasks.Any())
            {
                return NotFound("Задач нет");
            }

            return Ok(tasks);
        }

        [HttpPost]
        public async Task<ActionResult<ModelsTask>> PostTask([FromBody] TaskCreateRequest request)
        {
            // Проверяем существование пользователя
            var creator = await _context.Users.FindAsync(request.CreatorId);
            if (creator == null)
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
                Position = request.Position
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Создаем стандартные подзадачи
            _context.ChaptersSubtask.Add(new ChapterSubtask { Name = "Новые", TaskId = task.Id });
            _context.ChaptersSubtask.Add(new ChapterSubtask { Name = "В работе", TaskId = task.Id });
            _context.ChaptersSubtask.Add(new ChapterSubtask { Name = "Выполненные", TaskId = task.Id });
            await _context.SaveChangesAsync();

            // Добавляем ответственных и отправляем уведомления
            if (request.AssigneeIds != null && request.AssigneeIds.Any())
            {
                var assignerName = $"{creator.LastName} {creator.FirstName}";

                foreach (var assigneeId in request.AssigneeIds)
                {
                    // Проверяем существование пользователя
                    var assignee = await _context.Users.FindAsync(assigneeId);
                    if (assignee == null) continue;

                    // Добавляем ответственного
                    _context.TaskResponsibles.Add(new TaskResponsible
                    {
                        TaskId = task.Id,
                        UserId = assigneeId
                    });

                    // Отправляем уведомление, если есть FCM токен
                    if (!string.IsNullOrEmpty(assignee.FCMToken))
                    {
                        try
                        {
                            var notificationData = new Dictionary<string, string>
                            {
                                { "type", "task_assignment" },
                                { "taskId", task.ToString() },
                                { "taskName", task.Name },
                                { "assigner", assignerName }
                            };
                            await NotificationMobile.SendPushNotification(
                                fcmToken: assignee.FCMToken,
                                data: notificationData
                            );
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка при отправке уведомления пользователю {assigneeId}: {ex.Message}");
                        }
                    }
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
            public int Position { get; set; }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskUpdateRequest request)
        {
            // Получаем текущего пользователя из токена
            var currentUserId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            var creator = await _context.Users.FindAsync(currentUserId);

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            // Обновляем основные поля
            task.Name = request.Name ?? task.Name;
            task.Description = request.Description ?? task.Description;
            task.Chapter = request.ChapterId;

            var assignerName = $"{creator.LastName} {creator.FirstName}";

            // Обновляем ответственных
            if (request.AssigneeIds != null)
            {


                // Текущие
                var existingResponsibles = await _context.TaskResponsibles
                    .Where(tr => tr.TaskId == id)
                    .ToListAsync();

                // Удаляем участников, которых нет в новом списке
                var responsiblesToRemove = existingResponsibles
                    .Where(cp => !request.AssigneeIds.Contains(cp.UserId))
                    .ToList();

                foreach (var responsibles in responsiblesToRemove)
                {
                    var user = await _context.Users.FindAsync(responsibles.UserId);
                    if (user?.FCMToken != null)
                    {
                        var notificationDataRemove = new Dictionary<string, string>
                        {
                              { "type", "task_assignment_remove" },
                              { "taskId", task.ToString() },
                              { "taskName", task.Name },
                              { "assigner", assignerName }
                        };
                        await NotificationMobile.SendPushNotification(
                            fcmToken: user.FCMToken,
                            data: notificationDataRemove
                        );
                    }
                }

                _context.TaskResponsibles.RemoveRange(responsiblesToRemove);

                // Добавляем новых
                foreach (var assigneeId in request.AssigneeIds)
                {
                    // Проверяем существование пользователя
                    var assignee = await _context.Users.FindAsync(assigneeId);
                    if (assignee == null) continue;

                    if(!existingResponsibles.Any(cp => cp.UserId == assigneeId))
                    {
                        _context.TaskResponsibles.Add(new TaskResponsible
                        {
                            TaskId = id,
                            UserId = assigneeId
                        });

                        if (!string.IsNullOrEmpty(assignee.FCMToken))
                        {
                            try
                            {
                                var notificationData = new Dictionary<string, string>
                            {
                                { "type", "task_assignment" },
                                { "taskId", task.ToString() },
                                { "taskName", task.Name },
                                { "assigner", assignerName }
                            };
                                await NotificationMobile.SendPushNotification(
                                    fcmToken: assignee.FCMToken,
                                    data: notificationData
                                );
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Ошибка при отправке уведомления пользователю {assigneeId}: {ex.Message}");
                            }
                        }
                    }
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

        [HttpPut("update-order")]
        public async Task<IActionResult> UpdateTaskOrder([FromBody] UpdateTaskOrderRequest request)
        {
            if (request == null || request.TaskOrder == null || !request.TaskOrder.Any())
            {
                return BadRequest("Invalid request data");
            }

            var taskIds = request.TaskOrder.Select(t => t.TaskId).ToList();
            var tasks = await _context.Tasks
                .Where(t => taskIds.Contains(t.Id) && t.ProjectId == request.ProjectId)
                .ToListAsync();

            if (tasks.Count != taskIds.Count)
            {
                return BadRequest("Some tasks do not belong to the specified project");
            }

            foreach (var item in request.TaskOrder)
            {
                var task = tasks.FirstOrDefault(t => t.Id == item.TaskId);
                if (task != null)
                {
                    task.Position = item.Position;
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

        public class UpdateTaskOrderRequest
        {
            public int ProjectId { get; set; }
            public int ColumnId { get; set; }
            public List<TaskOrderItem> TaskOrder { get; set; }
        }

        public class TaskOrderItem
        {
            public int TaskId { get; set; }
            public int Position { get; set; }
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
