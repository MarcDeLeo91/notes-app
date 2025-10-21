using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesApi.Services;
using System.Security.Claims;
using NotesApi.Data;
using NotesApi.Models;

namespace NotesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AiController : ControllerBase
    {
        private readonly AiService _aiService;
        private readonly PlannerService _planner;
        private readonly ExecutorService _executor;
        private readonly AppDbContext _context;

        public AiController(AiService aiService, PlannerService planner, ExecutorService executor, AppDbContext context)
        {
            _aiService = aiService;
            _planner = planner;
            _executor = executor;
            _context = context;
        }

        private int GetUserId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(idStr) || !int.TryParse(idStr, out var id))
            {
                throw new UnauthorizedAccessException("User id missing or invalid.");
            }
            return id;
        }

        [HttpPost("execute")]
        public async Task<IActionResult> ExecutePrompt([FromBody] PromptRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Prompt)) return BadRequest("Falta 'prompt'");

            if (!_aiService.IsPromptAllowed(req.Prompt))
                return BadRequest("Prompt no permitido");

            // Generar plan
            var planJson = _planner.GeneratePlan(req.Prompt);
            var steps = _planner.ParsePlan(planJson);

            // Ejecutar plan
            var results = await _executor.ExecutePlan(GetUserId(), steps);

            // Guardar logs mínimos (puedes extender)
            var agentTask = new NotesApi.Models.AgentTask
            {
                UserId = GetUserId().ToString(),
                Prompt = req.Prompt,
                PlanJson = planJson,
                Status = "done",
                CreatedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow
            };
            _context.AgentTasks.Add(agentTask);
            await _context.SaveChangesAsync();

            return Ok(new { plan = planJson, results });
        }

        // Endpoint público para verificar que la API y Swagger funcionan sin token
        [AllowAnonymous]
        [HttpGet("health")]
        public IActionResult Health() => Ok(new { status = "ok", now = DateTime.UtcNow });
    }

    public class PromptRequest { public string Prompt { get; set; } = string.Empty; }
}
