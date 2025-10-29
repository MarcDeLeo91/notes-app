using System.ComponentModel.DataAnnotations;

namespace NotesApi.Models
{
    public class Curso
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Codigo { get; set; } = string.Empty;

        public int Creditos { get; set; }

        // Relación con profesor
        public int ProfesorId { get; set; }
        public Profesor? Profesor { get; set; }

        public ICollection<Inscripcion>? Inscripciones { get; set; }
    }
}
