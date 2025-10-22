using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesApi.Models;
using NotesApi.Services;
using System.Security.Claims;

namespace NotesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requiere autenticación para todas las rutas
    public class NotesController : ControllerBase
    {
        private readonly NoteService _noteService;

        public NotesController(NoteService noteService) => _noteService = noteService;

        /// <summary>
        /// Obtiene el ID del usuario autenticado desde el token JWT.
        /// </summary>
        /// <returns>ID del usuario</returns>
        /// <exception cref="UnauthorizedAccessException">Si el ID no es válido o está ausente</exception>
        private int GetUserId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(idStr) || !int.TryParse(idStr, out var id))
            {
                throw new UnauthorizedAccessException("User ID missing or invalid.");
            }
            return id;
        }

        /// <summary>
        /// Obtiene todas las notas del usuario autenticado.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetNotes()
        {
            try
            {
                var notes = await _noteService.GetNotes(GetUserId());
                return Ok(notes);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una nota específica del usuario autenticado.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNote(int id)
        {
            try
            {
                var note = await _noteService.GetNote(GetUserId(), id);
                if (note == null) return NotFound(new { message = "Nota no encontrada." });
                return Ok(note);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva nota para el usuario autenticado.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateNote([FromBody] Note note)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdNote = await _noteService.CreateNote(GetUserId(), note);
                return CreatedAtAction(nameof(GetNote), new { id = createdNote.Id }, createdNote);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una nota existente del usuario autenticado.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNote(int id, [FromBody] Note note)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _noteService.UpdateNote(GetUserId(), id, note);
                return updated
                    ? Ok(new { message = "Nota actualizada exitosamente." })
                    : NotFound(new { message = "Nota no encontrada." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una nota existente del usuario autenticado.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            try
            {
                var deleted = await _noteService.DeleteNote(GetUserId(), id);
                return deleted
                    ? Ok(new { message = "Nota eliminada exitosamente." })
                    : NotFound(new { message = "Nota no encontrada." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}