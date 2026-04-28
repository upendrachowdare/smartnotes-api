using System;

namespace SmartNotes.Api.Models
{
    public class Note
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string? Summary { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
