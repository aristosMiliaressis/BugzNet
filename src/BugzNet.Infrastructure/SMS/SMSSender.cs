using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace BugzNet.Infrastructure.SMS
{
    public class SMSSender : ISmsSender
    {
        public SMSSender(IOptions<SMSOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public SMSOptions Options { get; }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }

        public Task SendSmsAsync(string number, string message)
        {
            // // Plug in your SMS service here to send a text message.
            // // Your Account SID from twilio.com/console
            // var accountSid = Options.SMSAccountIdentification;
            // // Your Auth Token from twilio.com/console
            // var authToken = Options.SMSAccountPassword;

            // TwilioClient.Init(accountSid, authToken);

            // return MessageResource.CreateAsync(
            //     to: new PhoneNumber(number),
            //     from: new PhoneNumber(Options.SMSAccountFrom),
            //     body: message);

            return Task.FromResult(0);
        }
    }
}