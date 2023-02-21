using BugzNet.Core.Constants;
using BugzNet.Core.Entities;
using BugzNet.Core.Entities.Identity;
using BugzNet.Core.Utilities;
using BugzNet.Infrastructure.DataEF;
using BugzNet.Infrastructure.SMS;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BugzNet.Application.Requests.MyAccount.Commands
{
    public class GenerateOtpCommand : IRequest<bool>
    {
        public string UserEmail { get; set; }
        public string OTP { get; set; }
    }

    public class GenerateOtpCommandHandler : IRequestHandler<GenerateOtpCommand, bool>
    {
        private ISmsSender _smsSender;
        private BugzNetDataContext _context;

        public GenerateOtpCommandHandler(ISmsSender smsSender, BugzNetDataContext context)
        {
            _smsSender = smsSender;
            _context = context;
        }

        public async Task<bool> Handle(GenerateOtpCommand message, CancellationToken token)
        {
            var code = OtpGenerator.Generate();

            var user = await _context.Users
                                    .Where(i => i.Email == message.UserEmail)
                                    .FirstAsync(token);

            await _smsSender.SendSmsAsync(user.PhoneNumber, $"Your otp is {code}");

            var otp = new OTP()
            {
                Code = code,
                UserId = user.Id
            };

            _context.OTP.Add(otp);

            await _context.SaveChangesAsync(token);

            var logger = LoggingUtility.LoggerFactory.CreateLogger("DEBUG");
            logger.LogWarning("OTP: "+code);

            return true;
        }
    }
}
