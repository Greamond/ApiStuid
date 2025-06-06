﻿using ApiStuid.DbWork;
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
    public class ChaptersTaskController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public ChaptersTaskController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Chapters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChapterTask>>> GetChapters()
        {
            return await _context.ChaptersTask.ToListAsync();
        }

        // GET: api/Chapters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChapterTask>> GetChapter(int id)
        {
            var chapter = await _context.ChaptersTask.FindAsync(id);

            if (chapter == null)
            {
                return NotFound();
            }

            return chapter;
        }

        // POST: api/Chapters
        [HttpPost]
        public async Task<ActionResult<ChapterTask>> PostChapter(ChapterTask chapter)
        {
            _context.ChaptersTask.Add(chapter);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetChapter), new { id = chapter.Id }, chapter);
        }

        // PUT: api/Chapters/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateChapter(int id, ChapterTask chapter)
        {
            if (id != chapter.Id)
                return BadRequest();

            var existing = await _context.ChaptersTask.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Name = chapter.Name;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(existing);
            }
            catch
            {
                return StatusCode(500, "Ошибка сохранения");
            }
        }

        // DELETE: api/Chapters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChapter(int id)
        {
            var chapter = await _context.ChaptersTask.FindAsync(id);
            if (chapter == null)
                return NotFound();

            _context.ChaptersTask.Remove(chapter);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<IEnumerable<ChapterTask>>> GetChaptersByProject(int projectId)
        {
            var chapters = await _context.ChaptersTask
                .Where(c => c.ProjectId == projectId)
                .ToListAsync();

            if (chapters == null || !chapters.Any())
            {
                return NotFound("Нет колонок для этого проекта");
            }

            return chapters;
        }

        private bool ChapterExists(int id)
        {
            return _context.ChaptersTask.Any(e => e.Id == id);
        }
    }
}
