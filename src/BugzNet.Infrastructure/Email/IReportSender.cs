using BugzNet.Infrastructure.Exporting.Models;

namespace BugzNet.Infrastructure.Email
{
    public interface IReportSender
    {
        void Send(ExportDto[] exports, string subject, string body, string[] recipients);
        void SendLink(string subject, string body, string recipient);
    }
}
