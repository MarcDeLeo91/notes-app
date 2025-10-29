using System.ComponentModel.DataAnnotations;

namespace NotesApi.Models
{
    public class Note
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Relación con usuario
        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}
