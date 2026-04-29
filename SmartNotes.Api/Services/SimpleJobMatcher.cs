using System;
using System.Linq;
using SmartNotes.Api.Models;

namespace SmartNotes.Api.Services
{
    public class SimpleJobMatcher : IJobMatcher
    {
        private readonly string[] _keywords = new[] { ".net", "asp.net", "c#", "azure", "microservices", "devops" };

        public double Score(JobPosting job)
        {
            var text = (job.Title + " " + job.Description + " " + job.Company).ToLowerInvariant();
            var matches = _keywords.Count(k => text.Contains(k));
            var score = Math.Min(1.0, matches / (double)_keywords.Length);
            return score;
        }
    }
}
