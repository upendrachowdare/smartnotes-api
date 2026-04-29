using System;

namespace SmartNotes.Api.Models
{
    public class ApplicationAttempt
    {
        public int Id { get; set; }
        public string JobId { get; set; } = string.Empty;
        public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
        public bool Success { get; set; }
        public string? Details { get; set; }
    }
}
