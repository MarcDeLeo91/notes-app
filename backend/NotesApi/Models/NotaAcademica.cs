using System.ComponentModel.DataAnnotations;

namespace NotesApi.Models
{
    public class NotaAcademica
    {
        public int Id { get; set; }

        [Range(0, 5)]
        public decimal Valor { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public int InscripcionId { get; set; }
        public Inscripcion? Inscripcion { get; set; }
    }
}
