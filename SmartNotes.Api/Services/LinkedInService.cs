using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SmartNotes.Api.Services
{
    public interface ILinkedInService
    {
        Task<bool> SharePostAsync(string text);
    }

    public class LinkedInService : ILinkedInService
    {
        private readonly HttpClient _http;
        private readonly string _accessToken;

        public LinkedInService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _accessToken = config["LinkedIn:AccessToken"];
        }

        // Note: This is a simplified example. Real LinkedIn posting requires organization or person URN and additional steps.
        public async Task<bool> SharePostAsync(string text)
        {
            if (string.IsNullOrEmpty(_accessToken)) return false;

            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var payload = new
            {
                author = "urn:li:person:REPLACE_WITH_PERSON_URN",
                lifecycleState = "PUBLISHED",
                specificContent = new
                {
                    "com.linkedin.ugc.ShareContent" = new
                    {
                        shareCommentary = new { text },
                        shareMediaCategory = "NONE"
                    }
                },
                visibility = new { "com.linkedin.ugc.MemberNetworkVisibility" = "PUBLIC" }
            };

            var json = JsonSerializer.Serialize(payload);
            var response = await _http.PostAsync("https://api.linkedin.com/v2/ugcPosts", new StringContent(json, Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }
    }
}
