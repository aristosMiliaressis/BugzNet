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
using BugzNet.Core.Utilities;
using BugzNet.Application.Requests.Identity.Commands;
using System.Text;
using Newtonsoft.Json;
using System;
using BugzNet.Infrastructure.Configuration;

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
        private readonly AppConfig _config;

        public OtpVerificationQueryHandler(BugzNetDataContext db, SignInManager<BugzUser> signInManager, AppConfig config)
        {
            _signInManager = signInManager;
            _context = db;
            _config = config;
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

            var cookie = _signInManager.Context.Request.Cookies[AuthState.CookieName];
            var value = cookie.Split(".")[0];
            var state = (AuthState)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(Convert.FromBase64String(value)), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });

            state = new IdentityVerified();

            var val = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(state, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects })));
            var signature = CryptoUtility.Sign(val, _config.HMACSecret);

            _signInManager.Context.Response.Cookies.Append(AuthState.CookieName, $"{val}.{signature}");

            return true;
        }
    }
}
