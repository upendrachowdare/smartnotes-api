using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace SmartNotes.Api.Services
{
    public class DailyLinkedInPoster : BackgroundService
    {
        private readonly IOpenAiService _ai;
        private readonly ILinkedInService _linkedin;
        private readonly ILogger<DailyLinkedInPoster> _logger;
        private readonly TimeSpan _postTime;

        public DailyLinkedInPoster(IOpenAiService ai, ILinkedInService linkedin, IConfiguration config, ILogger<DailyLinkedInPoster> logger)
        {
            _ai = ai;
            _linkedin = linkedin;
            _logger = logger;

            // Read desired daily post time from configuration, fallback to 9:00 AM local time
            if (!TimeSpan.TryParse(config["DailyPost:Time"], out _postTime))
            {
                _postTime = new TimeSpan(9, 0, 0);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DailyLinkedInPoster started, posting at {time} local time.", _postTime);

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var next = new DateTime(now.Year, now.Month, now.Day, _postTime.Hours, _postTime.Minutes, _postTime.Seconds);
                if (next <= now) next = next.AddDays(1);

                var delay = next - now;
                _logger.LogInformation("Waiting {delay} until next post.", delay);
                await Task.Delay(delay, stoppingToken);

                try
                {
                    // generate a post about .NET Core, Azure AI, DevOps or Microservices
                    var topic = GetRandomTopic();
                    var prompt = $"Write a concise LinkedIn post about {topic} that is informative and professional, 2-4 short sentences.";
                    var post = await _ai.SummarizeAsync(prompt); // using SummarizeAsync as a general text generator

                    var success = await _linkedin.SharePostAsync(post);
                    _logger.LogInformation("Posted to LinkedIn: {success}", success);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create or post LinkedIn content.");
                }
            }
        }

        private static string GetRandomTopic()
        {
            var topics = new[] { 
                ".NET Core best practices", "Azure AI integration patterns", "DevOps automation tips", "Designing microservices" };
            var idx = new Random().Next(topics.Length);
            return topics[idx];
        }
    }
}
