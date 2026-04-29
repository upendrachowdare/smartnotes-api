using System.Threading.Tasks;
using SmartNotes.Api.Models;

namespace SmartNotes.Api.Services
{
    public interface IProviderApplyService
    {
        Task<bool> ApplyAsync(JobPosting job, string coverLetter);
    }
}
