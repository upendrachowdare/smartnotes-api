using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SmartNotes.Api.Models;

namespace SmartNotes.Api.Services
{
    public class JobPoller : BackgroundService
    {
        private readonly IEnumerable<IJobSource> _sources;
        private readonly IJobMatcher _matcher;
        private readonly IOpenAiService _ai;
        private readonly IJobApplyService _apply;
        private readonly ILogger<JobPoller> _logger;
        private readonly TimeSpan _interval;

        public JobPoller(IEnumerable<IJobSource> sources, IJobMatcher matcher, IOpenAiService ai, IJobApplyService apply, IConfiguration config, ILogger<JobPoller> logger)
        {
            _sources = sources;
            _matcher = matcher;
            _ai = ai;
            _apply = apply;
            _logger = logger;

            if (!int.TryParse(config["JobPollIntervalMinutes"], out var minutes)) minutes = 20;
            _interval = TimeSpan.FromMinutes(minutes);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("JobPoller started. Polling every {minutes} minutes.", _interval.TotalMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var jobs = new List<JobPosting>();

                    foreach (var src in _sources)
                    {
                        var fetched = await src.FetchAsync();
                        jobs.AddRange(fetched);
                    }

                    _logger.LogInformation("Fetched {count} jobs", jobs.Count);

                    foreach (var job in jobs)
                    {
                        var score = _matcher.Score(job);
                        _logger.LogInformation("Job {title} scored {score}", job.Title, score);

                        if (score >= 0.8)
                        {
                            var cover = await _ai.GenerateCoverLetterAsync(job);
                            // For now, do semi-automatic: log and store, and attempt apply via configured connector
                            _logger.LogInformation("Generated cover letter for {title}: {cover}", job.Title, cover);

                            var applied = await _apply.ApplyAsync(job, cover);
                            _logger.LogInformation("Attempted apply for {title}: {applied}", job.Title, applied);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during job polling");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
