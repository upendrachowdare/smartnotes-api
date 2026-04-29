using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using SmartNotes.Api.Models;

namespace SmartNotes.Api.Services
{
    public class SimpleJobMatcher : IJobMatcher
    {
        private readonly string[] _keywords;

        public SimpleJobMatcher(IConfiguration config)
        {
            // Default keywords include user's requested skills
            var defaults = new[] { ".net core", "mvc", "angular", "react", "reactjs", ".net", "c#", "asp.net", "efcore", "wcf", "webservices", "sqlserver", "oracle", "microservices", "azure", "azure ai", "git", "devops" };

            var configured = config["JobMatch:Keywords"];
            if (!string.IsNullOrWhiteSpace(configured))
            {
                _keywords = configured.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(k => k.ToLowerInvariant())
                    .ToArray();
            }
            else
            {
                _keywords = defaults;
            }
        }

        public double Score(JobPosting job)
        {
            var text = (job.Title + " " + job.Description + " " + job.Company + " " + job.Location).ToLowerInvariant();
            var matches = _keywords.Count(k => text.Contains(k));

            if (_keywords.Length == 0) return 0.0;
            var score = Math.Min(1.0, matches / (double)_keywords.Length);
            return score;
        }
    }
}
