using System.ComponentModel.DataAnnotations;

namespace NotesApi.Models
{
    public class Profesor
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Departamento { get; set; } = string.Empty;

        public ICollection<Curso>? Cursos { get; set; }
    }
}
