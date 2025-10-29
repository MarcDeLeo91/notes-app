using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace NotesApi.Utilities
{
    public class PromptMapResult
    {
        public bool IsValid { get; set; }
        public string? CommandKey { get; set; }
        public Dictionary<string, string>? Parameters { get; set; }
        public string? Reason { get; set; }
    }

    public static class PromptMapper
    {
        // Definimos patterns permitidos y su commandKey
        // Puedes extenderlos o afinarlos según necesites.
        private static readonly (Regex pattern, string commandKey)[] Patterns = new[]
        {
            // inscribir <email> en <codigo>
            (new Regex(@"inscribir\s+([^\s@]+@[^\s@]+\.[^\s@]+)\s+en\s+([A-Za-z0-9\-]+)", RegexOptions.IgnoreCase), "inscribir_estudiante_en_curso"),

            // agregar nota <valor> a <email> en <codigo>
            (new Regex(@"agregar\s+nota\s+([0-9]+(?:[.,][0-9]+)?)\s+a\s+([^\s@]+@[^\s@]+\.[^\s@]+)\s+en\s+([A-Za-z0-9\-]+)", RegexOptions.IgnoreCase), "agregar_nota"),

            // listar estudiantes en <codigo>
            (new Regex(@"listar\s+estudiantes\s+en\s+([A-Za-z0-9\-]+)", RegexOptions.IgnoreCase), "listar_estudiantes_por_curso"),

            // promedio de <email>
            (new Regex(@"promedio\s+de\s+([^\s@]+@[^\s@]+\.[^\s@]+)", RegexOptions.IgnoreCase), "calcular_promedio_estudiante"),

            // crear estudiante <nombre> <apellido> email <email>
            (new Regex(@"crear\s+estudiante\s+([A-Za-z]+)\s+([A-Za-z]+)\s+email\s+([^\s@]+@[^\s@]+\.[^\s@]+)", RegexOptions.IgnoreCase), "crear_estudiante")
        };

        public static PromptMapResult ValidateAndMap(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                return new PromptMapResult { IsValid = false, Reason = "Prompt vacío" };

            foreach (var (pattern, commandKey) in Patterns)
            {
                var m = pattern.Match(prompt);
                if (m.Success)
                {
                    var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    // Mapear según commandKey con grupos capturados
                    switch (commandKey)
                    {
                        case "inscribir_estudiante_en_curso":
                            parameters["email"] = m.Groups[1].Value;
                            parameters["cursoCodigo"] = m.Groups[2].Value;
                            break;
                        case "agregar_nota":
                            // normalizar coma a punto
                            var rawVal = m.Groups[1].Value.Replace(',', '.');
                            parameters["valor"] = rawVal;
                            parameters["email"] = m.Groups[2].Value;
                            parameters["cursoCodigo"] = m.Groups[3].Value;
                            break;
                        case "listar_estudiantes_por_curso":
                            parameters["cursoCodigo"] = m.Groups[1].Value;
                            break;
                        case "calcular_promedio_estudiante":
                            parameters["email"] = m.Groups[1].Value;
                            break;
                        case "crear_estudiante":
                            parameters["nombre"] = m.Groups[1].Value;
                            parameters["apellido"] = m.Groups[2].Value;
                            parameters["email"] = m.Groups[3].Value;
                            break;
                    }

                    return new PromptMapResult
                    {
                        IsValid = true,
                        CommandKey = commandKey,
                        Parameters = parameters
                    };
                }
            }

            return new PromptMapResult { IsValid = false, Reason = "No coincide con ningún prompt permitido" };
        }

        // Método utilitario para devolver la lista de prompts permitidos (útil en UI)
        public static IEnumerable<string> GetAllowedPrompts() => new[]
        {
            "inscribir <email> en <codigo>",
            "agregar nota <valor> a <email> en <codigo>",
            "listar estudiantes en <codigo>",
            "promedio de <email>",
            "crear estudiante <nombre> <apellido> email <email>"
        };
    }
}
