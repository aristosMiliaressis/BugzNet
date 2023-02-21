using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using BugzNet.Infrastructure.Configuration;
using BugzNet.Infrastructure.Exporting.Models;
using System;
using System.IO;

namespace BugzNet.Infrastructure.Email
{
    public class SmtpReportSender : IReportSender
    {
        private readonly SmtpSettings _settings;
        private readonly EmailSender _sender;

        public SmtpReportSender(SmtpSettings settings, EmailSender sender)
        {
            _settings = settings;
            _sender = sender;
        }

        public void Send(ExportDto[] exports, string subject, string body, string[] recipients)
        {
            if (recipients == null || recipients.Length < 1)
                throw new Exception();

            var sender = new MailboxAddress(_sender.Address, _sender.Address);
            var message = new MimeMessage();
            message.From.Add(sender);
            message.Sender = sender;
            foreach (var recipient in recipients)
                message.To.Add(MailboxAddress.Parse(recipient));

            message.Subject = subject;

            var multipart = new Multipart("mixed");
            var bodyText = new TextPart("plain")
            {
                Text = body ?? string.Empty
            };

            multipart.Add(bodyText);

            foreach (var export in exports)
            {
                var attachment = new MimePart(export.ContentType.Split('/')[0], export.ContentType.Split('/')[1])
                {
                    Content = new MimeContent(new MemoryStream(export.Bytes)),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = export.Name
                };

                multipart.Add(attachment);
            }

            message.Body = multipart;

            using (var client = new SmtpClient())
            {
                client.CheckCertificateRevocation = false;
                client.Connect(_settings.Host, _settings.Port);
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Authenticate(_sender.Address, _sender.Password);

                client.Send(message);
                client.Disconnect(true);
            }
        }

        public void SendLink(string subject, string body, string recipient)
        {
            if (recipient == null)
                throw new Exception();

            var sender = new MailboxAddress(_sender.Address, _sender.Address);
            var message = new MimeMessage();
            message.From.Add(sender);
            message.Sender = sender;
            message.To.Add(MailboxAddress.Parse(recipient));

            message.Subject = subject;

            var multipart = new Multipart("mixed");
            var bodyText = new TextPart("plain")
            {
                Text = body ?? string.Empty
            };

            multipart.Add(bodyText);

            message.Body = multipart;

            using (var client = new SmtpClient())
            {
                client.CheckCertificateRevocation = false;
                client.Connect(_settings.Host, _settings.Port);
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Authenticate(_sender.Address, _sender.Password);

                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
