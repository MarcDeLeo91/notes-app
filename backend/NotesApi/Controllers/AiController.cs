using Microsoft.AspNetCore.Mvc;
using NotesApi.Data;
using NotesApi.Models;
using NotesApi.Services;
using NotesApi.Utilities;

namespace NotesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly AiService _ai;
        private readonly PlannerService _planner;
        private readonly AppDbContext _db;
        private readonly ILogger<AiController> _logger;

        public AiController(AiService ai, PlannerService planner, AppDbContext db, ILogger<AiController> logger)
        {
            _ai = ai;
            _planner = planner;
            _db = db;
            _logger = logger;
        }

        [HttpPost("execute")]
        public async Task<IActionResult> ExecutePrompt([FromBody] PromptRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Prompt))
                return BadRequest(new { message = "Prompt vacío" });

            // Validar prompt permitido y extraer comando/params
            var map = PromptMapper.ValidateAndMap(req.Prompt);
            if (!map.IsValid)
                return BadRequest(map.Reason);

            // Crear tarea de agente
            var task = new AgentTask
            {
                Prompt = req.Prompt,
                Status = "pending",
                CreatedAt = DateTime.UtcNow,
                UserId = req.UserId
            };
            _db.AgentTasks.Add(task);
            await _db.SaveChangesAsync();

            _logger.LogInformation("AgentTask creado id={TaskId} command={Command}", task.Id, map.CommandKey);

            // Opcional: generar plan inmediatamente (el worker puede procesar después)
            var plan = await _planner.GeneratePlanAsync(req.Prompt);

            // Llamada simple a AiService para obtener texto / resumen
            var aiResult = await _ai.ExecuteAsync(req.Prompt);

            // Guardar plan JSON en la tarea
            task.PlanJson = System.Text.Json.JsonSerializer.Serialize(new { map.CommandKey, plan, aiResult });
            await _db.SaveChangesAsync();

            return Ok(new
            {
                taskId = task.Id,
                command = map.CommandKey,
                parameters = map.CommandKey,
                plan,
                aiResult
            });
        }

        public class PromptRequest { public string Prompt { get; set; } = ""; public int? UserId { get; set; } = null; }
    }
}
