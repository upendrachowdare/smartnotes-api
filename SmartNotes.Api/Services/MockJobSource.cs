using System.Collections.Generic;
using System.Threading.Tasks;
using SmartNotes.Api.Models;

namespace SmartNotes.Api.Services
{
    public class MockJobSource : IJobSource
    {
        public Task<IEnumerable<JobPosting>> FetchAsync()
        {
            var list = new List<JobPosting>
            {
                new JobPosting { Title = ".NET Developer", Company = "Contoso", Location = "Remote", Description = "Looking for .NET dev with ASP.NET Core experience.", Url = "https://contoso.example/jobs/1", Source = "Mock" },
                new JobPosting { Title = "Senior Backend Engineer", Company = "Fabrikam", Location = "New York, NY", Description = "Microservices and Azure experience required.", Url = "https://fabrikam.example/jobs/2", Source = "Mock" }
            };

            return Task.FromResult<IEnumerable<JobPosting>>(list);
        }
    }
}
