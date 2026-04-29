using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartNotes.Api.Models;

namespace SmartNotes.Api.Services
{
    // Greenhouse typically doesn't provide a generic apply API. This is a placeholder that logs the attempt.
    public class GreenhouseApplyService : IProviderApplyService
    {
        private readonly ILogger<GreenhouseApplyService> _logger;

        public GreenhouseApplyService(ILogger<GreenhouseApplyService> logger)
        {
            _logger = logger;
        }

        public Task<bool> ApplyAsync(JobPosting job, string coverLetter)
        {
            _logger.LogInformation("Greenhouse apply simulated for {job} at {company}", job.Title, job.Company);
            // For real apply, implement provider-specific API call or form submission.
            return Task.FromResult(false);
        }
    }
}
