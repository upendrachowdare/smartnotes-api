using System;

namespace SmartNotes.Api.Models
{
    public class JobPosting
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty; // e.g., Indeed, Dice, Greenhouse
        public DateTime PostedAt { get; set; } = DateTime.UtcNow;
    }
}
