using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartNotes.Api.Models;

namespace SmartNotes.Api.Services
{
    public class RssJobSource : IRssJobSource
    {
        private readonly HttpClient _http;
        private readonly ILogger<RssJobSource> _logger;
        private readonly string[] _feeds;

        public RssJobSource(HttpClient http, IConfiguration config, ILogger<RssJobSource> logger)
        {
            _http = http;
            _logger = logger;
            var feeds = config["Rss:Feeds"] ?? string.Empty;
            _feeds = feeds.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        public async Task<IEnumerable<JobPosting>> FetchAsync()
        {
            var results = new List<JobPosting>();
            foreach (var feed in _feeds)
            {
                try
                {
                    using var stream = await _http.GetStreamAsync(feed);
                    using var reader = XmlReader.Create(stream);
                    var synd = SyndicationFeed.Load(reader);
                    if (synd == null) continue;
                    foreach (var item in synd.Items)
                    {
                        var jp = new JobPosting
                        {
                            Id = item.Id ?? item.Links.FirstOrDefault()?.Uri?.ToString() ?? Guid.NewGuid().ToString(),
                            Title = item.Title.Text,
                            Company = item.Authors.FirstOrDefault()?.Name ?? string.Empty,
                            Location = string.Empty,
                            Description = item.Summary?.Text ?? string.Empty,
                            Url = item.Links.FirstOrDefault()?.Uri?.ToString() ?? string.Empty,
                            Source = "RSS",
                            PostedAt = item.PublishDate.UtcDateTime
                        };
                        results.Add(jp);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to read RSS feed {feed}", feed);
                }
            }

            return results;
        }
    }
}
