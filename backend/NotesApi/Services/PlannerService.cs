using System.Text.Json;
using System.Threading.Tasks;

namespace NotesApi.Services
{
    public class PlannerService
    {
        private readonly AiService _aiService;

        public PlannerService(AiService aiService)
        {
            _aiService = aiService;
        }

        public string GeneratePlan(string prompt)
        {
            if (!_aiService.IsPromptAllowed(prompt))
                throw new Exception("Prompt no permitido");

            var planJson = _aiService.GeneratePlan(prompt);

            // Validar JSON simple
            JsonDocument.Parse(planJson);

            return planJson;
        }

        public async Task<string> GeneratePlanAsync(string prompt)
        {
            return await Task.Run(() => GeneratePlan(prompt));
        }

        public List<PlanStep> ParsePlan(string planJson)
        {
            var doc = JsonDocument.Parse(planJson);
            var steps = new List<PlanStep>();

            foreach (var element in doc.RootElement.GetProperty("plan").EnumerateArray())
            {
                steps.Add(new PlanStep
                {
                    StepIndex = element.GetProperty("stepIndex").GetInt32(),
                    Action = element.GetProperty("action").GetString() ?? "",
                    ParametersJson = element.GetProperty("parameters").GetRawText(),
                    Description = element.GetProperty("description").GetString() ?? ""
                });
            }

            return steps;
        }
    }

    public class PlanStep
    {
        public int StepIndex { get; set; }
        public string Action { get; set; } = string.Empty;
        public string ParametersJson { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
