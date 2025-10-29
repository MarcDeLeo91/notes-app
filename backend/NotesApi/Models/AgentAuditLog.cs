namespace NotesApi.Models
{
    public class AgentAuditLog
    {
        public int Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? AgentTaskId { get; set; }
        public AgentTask? AgentTask { get; set; }
    }
}
