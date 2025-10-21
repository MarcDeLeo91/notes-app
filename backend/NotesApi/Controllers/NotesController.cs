using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesApi.Models;
using NotesApi.Services;
using System.Security.Claims;

namespace NotesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly NoteService _noteService;
        public NotesController(NoteService noteService) => _noteService = noteService;

        private int GetUserId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(idStr) || !int.TryParse(idStr, out var id))
            {
                throw new UnauthorizedAccessException("User id missing or invalid.");
            }
            return id;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotes() => Ok(await _noteService.GetNotes(GetUserId()));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNote(int id)
        {
            var note = await _noteService.GetNote(GetUserId(), id);
            if (note == null) return NotFound();
            return Ok(note);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNote([FromBody] Note note) =>
            Ok(await _noteService.CreateNote(GetUserId(), note));

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNote(int id, [FromBody] Note note)
        {
            var updated = await _noteService.UpdateNote(GetUserId(), id, note);
            return updated ? Ok(new { message = "Nota actualizada" }) : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var deleted = await _noteService.DeleteNote(GetUserId(), id);
            return deleted ? Ok(new { message = "Nota eliminada" }) : NotFound();
        }
    }
}
