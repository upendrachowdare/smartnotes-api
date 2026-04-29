using System.Threading.Tasks;

namespace SmartNotes.Api.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
