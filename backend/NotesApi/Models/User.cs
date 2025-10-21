using System.Collections.Generic;

namespace NotesApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public ICollection<Note>? Notes { get; set; }
    }
}
