using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace BugzNet.Infrastructure.SMS
{
    public interface ISmsSender
    {
        SMSOptions Options { get; }

        Task SendEmailAsync(string email, string subject, string message);

        Task SendSmsAsync(string number, string message);
    }
}