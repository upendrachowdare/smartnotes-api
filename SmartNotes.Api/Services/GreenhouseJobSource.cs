using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartNotes.Api.Models;

namespace SmartNotes.Api.Services
{
    public class GreenhouseJobSource : IJobSource
    {
        private readonly HttpClient _http;
        private readonly string[] _companies;
        private readonly ILogger<GreenhouseJobSource> _logger;

        public GreenhouseJobSource(HttpClient http, IConfiguration config, ILogger<GreenhouseJobSource> logger)
        {
            _http = http;
            _logger = logger;

            var companies = config["Greenhouse:Companies"] ?? string.Empty;
            _companies = companies.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        public async Task<IEnumerable<JobPosting>> FetchAsync()
        {
            var results = new List<JobPosting>();

            if (_companies == null || _companies.Length == 0)
            {
                _logger.LogWarning("No Greenhouse companies configured (Greenhouse:Companies).");
                return results;
            }

            foreach (var company in _companies)
            {
                try
                {
                    var url = $"https://boards-api.greenhouse.io/v1/boards/{company}/jobs";
                    using var res = await _http.GetAsync(url);
                    if (!res.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("Greenhouse API returned {status} for {company}", res.StatusCode, company);
                        continue;
                    }

                    await using var stream = await res.Content.ReadAsStreamAsync();
                    using var doc = await JsonDocument.ParseAsync(stream);

                    if (!doc.RootElement.TryGetProperty("jobs", out var jobs)) continue;

                    foreach (var j in jobs.EnumerateArray())
                    {
                        var id = TryGetString(j, "id") ?? TryGetString(j, "internal_job_id") ?? Guid.NewGuid().ToString();
                        var title = TryGetString(j, "title") ?? TryGetString(j, "name") ?? string.Empty;
                        var location = string.Empty;
                        if (j.TryGetProperty("location", out var locEl))
                        {
                            location = TryGetString(locEl, "name") ?? TryGetString(locEl, "city") ?? string.Empty;
                        }

                        var description = TryGetString(j, "content") ?? TryGetString(j, "description") ?? string.Empty;
                        var urlJob = TryGetString(j, "absolute_url") ?? TryGetString(j, "url") ?? string.Empty;
                        var postedAt = DateTime.UtcNow;
                        if (j.TryGetProperty("created_at", out var createdEl) && createdEl.ValueKind == JsonValueKind.String)
                        {
                            if (DateTime.TryParse(createdEl.GetString(), out var dt)) postedAt = dt.ToUniversalTime();
                        }

                        var posting = new JobPosting
                        {
                            Id = $"greenhouse-{company}-{id}",
                            Title = title,
                            Company = company,
                            Location = location,
                            Description = description,
                            Url = urlJob,
                            Source = "Greenhouse",
                            PostedAt = postedAt
                        };

                        results.Add(posting);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching jobs from Greenhouse for {company}", company);
                }
            }

            return results;
        }

        private static string? TryGetString(JsonElement el, string propName)
        {
            if (el.ValueKind == JsonValueKind.Object && el.TryGetProperty(propName, out var p) && p.ValueKind == JsonValueKind.String)
                return p.GetString();
            return null;
        }
    }
}
