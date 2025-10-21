using System.Text.Json;

namespace NotesApi.Services
{
    public class ExecutorService
    {
        private readonly NoteService _noteService;

        public ExecutorService(NoteService noteService)
        {
            _noteService = noteService;
        }

        public async Task<List<ExecutionResult>> ExecutePlan(int userId, List<PlanStep> steps)
        {
            var results = new List<ExecutionResult>();

            foreach (var step in steps)
            {
                var r = new ExecutionResult { StepIndex = step.StepIndex, Action = step.Action, Success = false };
                try
                {
                    switch (step.Action)
                    {
                        case "archive_notes":
                            // ejemplo: podrías parsear parameters para elegir qué archivar
                            r.Message = "Acción archive_notes ejecutada (simulada)";
                            r.Success = true;
                            break;

                        case "delete_notes":
                            r.Message = "Acción delete_notes ejecutada (simulada)";
                            r.Success = true;
                            break;

                        default:
                            r.Message = $"Acción '{step.Action}' no implementada";
                            r.Success = false;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    r.Message = ex.Message;
                }

                results.Add(r);
            }

            return results;
        }
    }

    public class ExecutionResult
    {
        public int StepIndex { get; set; }
        public string Action { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
