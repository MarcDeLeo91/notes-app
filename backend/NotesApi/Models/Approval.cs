using System.ComponentModel.DataAnnotations;

namespace NotesApi.Models
{
    public class Approval
    {
        public int Id { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty; // e.g. "CreateNote", "DeleteStudent"

        public bool Approved { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? UserId { get; set; }
        public User? User { get; set; }

        public int? AgentTaskId { get; set; }
        public AgentTask? AgentTask { get; set; }
    }
}
