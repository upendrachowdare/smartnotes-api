using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SmartNotes.Api.Services
{
    public interface IOpenAiService
    {
        Task<string> SummarizeAsync(string text);
        Task<string> GenerateCoverLetterAsync(Models.JobPosting job);
    }

    public class OpenAiService : IOpenAiService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        public OpenAiService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _apiKey = config["OpenAI:ApiKey"] ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? string.Empty;
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("OpenAI API key not configured. Set OpenAI:ApiKey in appsettings or OPENAI_API_KEY environment variable.");
            }
        }

        public async Task<string> SummarizeAsync(string text)
        {
            var payload = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] { new { role = "user", content = $"Summarize the following note concisely:\n\n{text}" } },
                max_tokens = 300
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            using var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return content ?? string.Empty;
        }

        public async Task<string> GenerateCoverLetterAsync(Models.JobPosting job)
        {
            var prompt = $"Write a concise professional cover letter for the job:\nTitle: {job.Title}\nCompany: {job.Company}\nLocation: {job.Location}\nDescription: {job.Description}\n\nTailor the letter to emphasize .NET, Azure, and microservices experience. Keep it short (3-4 sentences).";

            var payload = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] { new { role = "user", content = prompt } },
                max_tokens = 300
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            using var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return content ?? string.Empty;
        }
    }
}
