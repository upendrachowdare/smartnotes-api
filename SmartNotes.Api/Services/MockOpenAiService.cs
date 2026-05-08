using System.Threading.Tasks;
using SmartNotes.Api.Models;

namespace SmartNotes.Api.Services
{
    public class MockOpenAiService : IOpenAiService
    {
        public Task<string> SummarizeAsync(string text)
        {
            // very simple mock summary
            var summary = text?.Length > 200 ? text.Substring(0, 197) + "..." : text ?? string.Empty;
            return Task.FromResult("[MOCK SUMMARY] " + summary);
        }

        public Task<string> GenerateCoverLetterAsync(JobPosting job)
        {
            var cover = $"Dear {job.Company},\n\nI am excited to apply for the {job.Title} role. I have strong experience with .NET, Azure, and microservices which align well with this opportunity.\n\nBest regards.";
            return Task.FromResult("[MOCK COVER] " + cover);
        }
    }
}
