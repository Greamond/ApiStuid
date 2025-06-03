using ApiStuid.DbWork;
using ApiStuid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiStuid.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChaptersSubtaskController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public ChaptersSubtaskController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Chapters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChapterSubtask>>> GetChapters()
        {
            return await _context.ChaptersSubtask.ToListAsync();
        }

        // GET: api/Chapters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChapterSubtask>> GetChapter(int id)
        {
            var chapter = await _context.ChaptersSubtask.FindAsync(id);

            if (chapter == null)
            {
                return NotFound();
            }

            return chapter;
        }

        // POST: api/Chapters
        [HttpPost]
        public async Task<ActionResult<ChapterSubtask>> PostChapter(ChapterSubtask chapter)
        {
            _context.ChaptersSubtask.Add(chapter);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetChapter), new { id = chapter.Id }, chapter);
        }

        // PUT: api/Chapters/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubtaskColumn(int id, [FromBody] UpdateChapterDto chapters)
        {
            var existing = await _context.ChaptersSubtask.FindAsync(id);

            if (existing == null)
            {
                return NotFound(new { Message = "Колонка не найдена" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            existing.Name = chapters.Name;

            try
            {
                await _context.SaveChangesAsync();

                // Возвращаем обновлённую колонку
                return Ok(existing);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Ошибка при сохранении изменений", Error = ex.Message });
            }
        }

        public class UpdateChapterDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int TaskId { get; set; }
        }

        // DELETE: api/Chapters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChapter(int id)
        {
            var chapter = await _context.ChaptersSubtask.FindAsync(id);
            if (chapter == null)
            {
                return NotFound();
            }

            _context.ChaptersSubtask.Remove(chapter);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("task/{taskId}")]
        public async Task<ActionResult<IEnumerable<ChapterSubtask>>> GetChaptersByTask(int taskId)
        {
            var chapters = await _context.ChaptersSubtask
                .Where(c => c.TaskId == taskId)
                .ToListAsync();

            if (chapters == null || !chapters.Any())
            {
                return NotFound("Нет колонок для этой задачи");
            }

            return chapters;
        }

        private bool ChapterExists(int id)
        {
            return _context.ChaptersSubtask.Any(e => e.Id == id);
        }
    }
}
