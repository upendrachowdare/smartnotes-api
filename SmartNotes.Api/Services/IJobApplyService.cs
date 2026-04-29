using System.Threading.Tasks;
using SmartNotes.Api.Models;

namespace SmartNotes.Api.Services
{
    public interface IJobApplyService
    {
        Task<bool> ApplyAsync(JobPosting job, string coverLetter);
    }
}
