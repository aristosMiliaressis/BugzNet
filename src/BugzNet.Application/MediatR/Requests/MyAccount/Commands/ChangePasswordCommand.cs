using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using BugzNet.Infrastructure.MediatR;
using BugzNet.Infrastructure.DataEF;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BugzNet.Core.Entities.Identity;
using BugzNet.Core.Constants;

namespace BugzNet.Application.Requests.MyAccount.Commands
{
    public class ChangePasswordCommand : IRequest<CommandResponse>
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string PasswordConfirmation { get; set; }
    }

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, CommandResponse>
    {
        private readonly BugzNetDataContext _context;
        private readonly UserManager<BugzUser> _userManager;
        private readonly SignInManager<BugzUser> _signInManager;
        private readonly string _email;

        public ChangePasswordCommandHandler(BugzNetDataContext context, UserManager<BugzUser> userManager, SignInManager<BugzUser> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;

            _email = httpContextAccessor.HttpContext.User.Identity.Name;
        }

        public async Task<CommandResponse> Handle(ChangePasswordCommand command, CancellationToken token)
        {
            BugzUser user = await _userManager.FindByEmailAsync(_email);

            IdentityResult result = await _userManager.ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword);
            if (!result.Succeeded)
            {
                return CommandResponse.WithErrors(result.Errors.Select(e => new CommandError(e.Code, e.Description)).ToArray());
            }

            var claim =  user.Claims?.FirstOrDefault(c => c.ClaimType == BugzClaims.MustChangePass);
            if (claim != null)
            {
                var res = await _userManager.RemoveClaimAsync(user, claim.ToClaim());
                if (!res.Succeeded)
                    throw new Exception("Failed to remove Must Change Pass claim: " + string.Join(Environment.NewLine, res.Errors.Select(e => $"{e.Code}: {e.Description}")));
            }
            user.MustChangePassword = false;
            _context.Users.Update(user);

            await _context.SaveChangesAsync(token);
            await _signInManager.SignOutAsync();

            return CommandResponse.Ok();
        }
    }

    public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordValidator()
        {
            RuleFor(e => e.NewPassword)
                .MinimumLength(8);

            RuleFor(e => e.PasswordConfirmation)
                .Equal(e => e.NewPassword)
                .WithMessage("Confirmation password did not match new password.");

            RuleFor(e => e.NewPassword)
                .NotEqual(e => e.CurrentPassword)
                .WithMessage("New password must be different from current one.");
        }
    }
}
