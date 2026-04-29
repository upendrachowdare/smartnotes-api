using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SmartNotes.Api.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public SmtpEmailSender(IConfiguration config)
        {
            _config = config;
        }

        public Task SendEmailAsync(string to, string subject, string body)
        {
            var host = _config["Smtp:Host"];
            var port = int.TryParse(_config["Smtp:Port"], out var p) ? p : 587;
            var user = _config["Smtp:User"]; // optional
            var pass = _config["Smtp:Pass"]; // optional
            var from = _config["Smtp:From"] ?? user ?? "noreply@example.com";

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = true,
                Credentials = string.IsNullOrEmpty(user) ? CredentialCache.DefaultNetworkCredentials : new NetworkCredential(user, pass)
            };

            var msg = new MailMessage(from, to, subject, body) { IsBodyHtml = false };
            client.Send(msg);
            return Task.CompletedTask;
        }
    }
}
