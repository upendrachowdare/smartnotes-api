using SmartNotes.Api.Models;

namespace SmartNotes.Api.Services
{
    public interface IJobMatcher
    {
        double Score(JobPosting job);
    }
}
