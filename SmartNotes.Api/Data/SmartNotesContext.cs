using Microsoft.EntityFrameworkCore;
using SmartNotes.Api.Models;

namespace SmartNotes.Api.Data
{
    public class SmartNotesContext : DbContext
    {
        public SmartNotesContext(DbContextOptions<SmartNotesContext> options) : base(options) { }

        public DbSet<JobPosting> JobPostings { get; set; }
        public DbSet<ApplicationAttempt> ApplicationAttempts { get; set; }
    }
}
