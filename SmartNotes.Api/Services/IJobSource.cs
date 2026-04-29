using System.Collections.Generic;
using System.Threading.Tasks;
using SmartNotes.Api.Models;

namespace SmartNotes.Api.Services
{
    public interface IJobSource
    {
        Task<IEnumerable<JobPosting>> FetchAsync();
    }
}
