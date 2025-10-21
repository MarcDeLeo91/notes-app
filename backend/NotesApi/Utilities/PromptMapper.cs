using System.Collections.Generic;

namespace NotesApi.Utilities
{
    public static class PromptMapper
    {
        private static readonly Dictionary<string, string> PromptMap = new()
        {
            { "crear nota rápida", "CREATE_QUICK" },
            { "crear nota: {titulo} | {contenido}", "CREATE_CUSTOM" },
            { "modificar nota {id}: {titulo} | {contenido}", "UPDATE" },
            { "eliminar nota {id}", "DELETE" },
            { "marcar favorita {id}", "TOGGLE_FAVORITE" },
            { "archivar nota {id}", "ARCHIVE" },
            { "desarchivar nota {id}", "UNARCHIVE" },
            { "agregar etiqueta {id}: {etiqueta}", "ADD_TAG" },
            { "buscar notas: {texto}", "SEARCH" },
            { "borrar todas las notas archivadas", "DELETE_ARCHIVED" }
        };

        public static string? GetCommandKey(string prompt)
        {
            if (prompt == null) return null;
            var lower = prompt.ToLower().Trim();
            return PromptMap.ContainsKey(lower) ? PromptMap[lower] : null;
        }
    }
}