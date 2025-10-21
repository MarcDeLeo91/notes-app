using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NotesApi.Models
{
    public class AgentTask
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string Prompt { get; set; } = string.Empty;

        public string? PlanJson { get; set; }   // Plan generado por PlannerService (mock o LLM)
        public string Status { get; set; } = "pending"; // pending, running, waiting_approval, done, failed
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        public ICollection<AgentAuditLog>? AuditLogs { get; set; }
    }
}
