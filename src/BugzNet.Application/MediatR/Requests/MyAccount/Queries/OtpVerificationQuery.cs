using BugzNet.Infrastructure.DataEF;
using BugzNet.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using BugzNet.Core.Entities.Identity;
using BugzNet.Core.Constants;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using BugzNet.Core.Localization;

namespace BugzNet.Application.Requests.MyAccount.Queries
{
    public class OtpVerificationQuery : IRequest<bool>
    {
        public string UserEmail { get; set; }
        public string OTP { get; set; }
    }

    public class OtpVerificationQueryHandler : IRequestHandler<OtpVerificationQuery, bool>
    {
        private readonly BugzNetDataContext _context;
        private readonly SignInManager<BugzUser> _signInManager;

        public OtpVerificationQueryHandler(BugzNetDataContext db, SignInManager<BugzUser> signInManager)
        {
            _signInManager = signInManager;
            _context = db;
        }

        public async Task<bool> Handle(OtpVerificationQuery message, CancellationToken token)
        {
            var user = await _context.Users
                                    .Include(u => u.Claims)
                                    .Include(u => u.UserRoles)
                                        .ThenInclude(u => u.Role)
                                    .Where(i => i.Email == message.UserEmail)
                                    .FirstAsync(token);

            var otp = await _context.OTP
                                    .Where(i => i.UserId == user.Id && i.Code == message.OTP)
                                    .FirstOrDefaultAsync(token);

            if (otp == null)
                return false;

            if (LocalizationUtility.LocalTime > otp.CreatedOn.Add(OTP.Expiration))
                return false;

            var claim = _signInManager.Context.User.Claims?.FirstOrDefault(c => c.Type == BugzClaims.SecondFactorRequied);
            if (claim != null)
            {
                var claimsPrincipal = (ClaimsPrincipal) _signInManager.Context.User;
                var identity = (ClaimsIdentity)claimsPrincipal.Identity;
                identity.RemoveClaim(claim);
                claimsPrincipal.AddIdentity(identity);

                await _signInManager.Context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
            }

            return true;
        }
    }
}
