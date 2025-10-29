using System.ComponentModel.DataAnnotations;

namespace NotesApi.Models
{
    public class Estudiante
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public DateTime FechaNacimiento { get; set; }

        public ICollection<Inscripcion>? Inscripciones { get; set; }
    }
}
