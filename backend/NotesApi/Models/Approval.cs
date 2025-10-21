using System;
using System.ComponentModel.DataAnnotations;

namespace NotesApi.Models
{
    public class Approval
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AgentTaskId { get; set; }
        public AgentTask? AgentTask { get; set; }

        public int StepIndex { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? Description { get; set; }

        public bool Approved { get; set; } = false;
        public bool Reviewed { get; set; } = false;
        public string? ReviewedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }
    }
}
