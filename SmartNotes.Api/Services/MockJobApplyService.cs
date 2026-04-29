using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartNotes.Api.Models;

namespace SmartNotes.Api.Services
{
    public class MockJobApplyService : IJobApplyService
    {
        private readonly ILogger<MockJobApplyService> _logger;

        public MockJobApplyService(ILogger<MockJobApplyService> logger)
        {
            _logger = logger;
        }

        public Task<bool> ApplyAsync(JobPosting job, string coverLetter)
        {
            _logger.LogInformation("Mock applying to {job} at {company}. Cover: {cover}", job.Title, job.Company, coverLetter);
            // In production, implement provider-specific apply logic.
            return Task.FromResult(true);
        }
    }
}
