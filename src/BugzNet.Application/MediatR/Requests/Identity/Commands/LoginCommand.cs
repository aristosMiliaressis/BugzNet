using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using BugzNet.Infrastructure.Email;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using BugzNet.Core.Constants;
using BugzNet.Core.Entities.Identity;
using Newtonsoft.Json;
using BugzNet.Core.Utilities;
using System.Text;
using BugzNet.Infrastructure.Configuration;

namespace BugzNet.Application.Requests.Identity.Commands
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(m => m.Email)
                .EmailAddress();

            RuleFor(m => m.Password)
                .NotNull()
                .NotEmpty();
        }
    }
    public class LoginCommand : IRequest<bool>
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
    public class LoginCommandHandler : IRequestHandler<LoginCommand, bool>
    {
        private readonly SignInManager<BugzUser> _signInManager;
        private readonly UserManager<BugzUser> _userManager;
        private readonly AppConfig _config;
        private readonly IReportSender _sender;

        public LoginCommandHandler(SignInManager<BugzUser> signInManager, UserManager<BugzUser> userManager, IReportSender sender, AppConfig config)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _config =  config;
            _sender = sender;
        }

        public async Task<bool> Handle(LoginCommand message, CancellationToken token)
        {
            var user = await _userManager.FindByEmailAsync(message.Email);
            if (user == null)
            {
                return false;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, message.Password, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    _sender.SendLink("User Account Locked Out", "Your account has been locked out due to consecutive failed login attempts.", message.Email);
                }
                return result.Succeeded;
            }

            var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
            if (claimsPrincipal?.Identity is ClaimsIdentity claimsIdentity)
            {
                AuthState state = null;

                if (user.MustChangePassword)
                {
                    state = new PasswordChangeRequired();
                }
                else if (user.TwoFactorEnabled)
                {
                    state = new VerificationRequired();
                }

                var val = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(state, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects })));
                var signature = CryptoUtility.Sign(val, _config.HMACSecret);

                _signInManager.Context.Response.Cookies.Append(AuthState.CookieName, $"{val}.{signature}");
            }
            
            await _signInManager.Context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return result.Succeeded;
        }
    }
    

    public interface AuthState
    {
        public static string CookieName = "BugzNet-AuthState";
    }

    public class VerificationRequired : AuthState
    {
    }

    public class PasswordChangeRequired : AuthState
    {
    }

    public class IdentityVerified : AuthState
    {
    }
}
