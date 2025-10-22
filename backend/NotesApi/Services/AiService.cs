using System.Text.Json;

namespace NotesApi.Services
{
    public class AiService
    {
        private readonly List<string> AllowedPrompts = new List<string>
        {
            "crear nota rápida",
            "crear nota: {titulo} | {contenido}",
            "modificar nota {id}: {titulo} | {contenido}",
            "eliminar nota {id}",
            "marcar favorita {id}",
            "archivar nota {id}",
            "desarchivar nota {id}",
            "agregar etiqueta {id}: {etiqueta}",
            "buscar notas: {texto}",
            "borrar todas las notas archivadas"
        };

        public bool IsPromptAllowed(string prompt) =>
            AllowedPrompts.Any(p => p.Equals(prompt.Trim(), StringComparison.OrdinalIgnoreCase));

        
        public string GeneratePlan(string prompt)
        {
            if (!IsPromptAllowed(prompt)) throw new Exception("Prompt no permitido");

            var plan = new
            {
                plan = new[]
                {
                    new { stepIndex = 1, action = "find_notes", parameters = new { }, description = "Buscar notas relevantes" }
                }
            };

            return JsonSerializer.Serialize(plan);
        }
    }
}
