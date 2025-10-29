using Microsoft.AspNetCore.Mvc;
using NotesApi.Services;

namespace NotesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotasController : ControllerBase
    {
        private readonly NotaService _service;
        public NotasController(NotaService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddNotaRequest req)
        {
            try
            {
                var nota = await _service.CreateAsync(new Models.NotaAcademica
                {
                    InscripcionId = req.InscripcionId,
                    Valor = req.Valor
                });
                return Ok(nota);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("estudiante/{estudianteId}")]
        public async Task<IActionResult> ByEstudiante(int estudianteId) => Ok(await _service.GetNotasByEstudianteAsync(estudianteId));

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        public class AddNotaRequest { public int InscripcionId { get; set; } public decimal Valor { get; set; } }
    }
}
