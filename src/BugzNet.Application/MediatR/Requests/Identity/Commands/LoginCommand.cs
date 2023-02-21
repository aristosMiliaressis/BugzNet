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
        private readonly IReportSender _sender;

        public LoginCommandHandler(SignInManager<BugzUser> signInManager, UserManager<BugzUser> userManager, IReportSender sender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
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
                if (user.MustChangePassword && !claimsIdentity.HasClaim(c => c.Type == BugzClaims.MustChangePass))
                {
                    var claim = new Claim(BugzClaims.MustChangePass, string.Empty);
                    claimsIdentity.AddClaim(claim);
                    await _userManager.AddClaimAsync(user, claim);
                }

                if (user.TwoFactorEnabled && !claimsIdentity.HasClaim(c => c.Type == BugzClaims.SecondFactorRequied))
                {
                    var claim = new Claim(BugzClaims.SecondFactorRequied, string.Empty);
                    claimsIdentity.AddClaim(claim);
                    await _userManager.AddClaimAsync(user, claim);
                }
            }
            
            await _signInManager.Context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return result.Succeeded;
        }
    }
}
