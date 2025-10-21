using System;
using System.ComponentModel.DataAnnotations;

namespace NotesApi.Models
{
    public class AgentAuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AgentTaskId { get; set; }
        public AgentTask? AgentTask { get; set; }

        public int StepIndex { get; set; }

        [Required]
        public string Action { get; set; } = string.Empty;

        public string? ParametersJson { get; set; }
        public string Result { get; set; } = "pending"; // pending, success, failed
        public string? Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
