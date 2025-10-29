using Microsoft.AspNetCore.Mvc;
using NotesApi.Services;

namespace NotesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InscripcionesController : ControllerBase
    {
        private readonly InscripcionService _service;
        public InscripcionesController(InscripcionService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> Inscribir([FromBody] InscribirRequest req)
        {
            try
            {
                var ins = await _service.CreateAsync(new Models.Inscripcion
                {
                    EstudianteId = req.EstudianteId,
                    CursoId = req.CursoId
                });
                return Ok(ins);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("curso/{cursoId}")]
        public async Task<IActionResult> ByCurso(int cursoId) => Ok(await _service.GetByCursoAsync(cursoId));

        [HttpGet("estudiante/{estudianteId}")]
        public async Task<IActionResult> ByEstudiante(int estudianteId) => Ok(await _service.GetByEstudianteAsync(estudianteId));

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var inscripciones = await _service.GetAllAsync();
                return Ok(inscripciones);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public class InscribirRequest { public int EstudianteId { get; set; } public int CursoId { get; set; } }
    }
}
