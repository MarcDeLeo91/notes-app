using System.ComponentModel.DataAnnotations;

namespace NotesApi.Models
{
    public class AgentTask
    {
        public int Id { get; set; }

        [Required]
        public string Prompt { get; set; } = string.Empty;

        public string? PlanJson { get; set; }
        public string Status { get; set; } = "pending"; // pending, running, completed, failed
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        // Relación con usuario (quién lo solicitó)
        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}
