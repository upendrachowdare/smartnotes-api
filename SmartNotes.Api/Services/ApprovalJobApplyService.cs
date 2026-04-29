using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SmartNotes.Api.Data;
using SmartNotes.Api.Models;

namespace SmartNotes.Api.Services
{
    public class ApprovalJobApplyService : IJobApplyService
    {
        private readonly SmartNotesContext _db;
        private readonly IEmailSender _email;
        private readonly ILogger<ApprovalJobApplyService> _logger;
        private readonly string _approvalEmail;

        public ApprovalJobApplyService(SmartNotesContext db, IEmailSender email, IConfiguration config, ILogger<ApprovalJobApplyService> logger)
        {
            _db = db;
            _email = email;
            _logger = logger;
            _approvalEmail = config["Approval:Email"] ?? "madamanchiupen@gmail.com";
        }

        public async Task<bool> ApplyAsync(JobPosting job, string coverLetter)
        {
            // Save attempt as pending
            var attempt = new ApplicationAttempt { JobId = job.Id, Success = false, Details = "Pending approval" };
            _db.ApplicationAttempts.Add(attempt);
            await _db.SaveChangesAsync();

            // Send approval email with job details and cover letter
            var subject = $"Approval needed to apply: {job.Title} at {job.Company}";
            var body = $"Job: {job.Title}\nCompany: {job.Company}\nUrl: {job.Url}\n\nCover Letter:\n{coverLetter}\n\nReply to approve or visit the dashboard to confirm.";
            try
            {
                await _email.SendEmailAsync(_approvalEmail, subject, body);
                _logger.LogInformation("Sent approval email to {email} for job {job}", _approvalEmail, job.Title);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Failed to send approval email");
            }

            return false;
        }
    }
}
