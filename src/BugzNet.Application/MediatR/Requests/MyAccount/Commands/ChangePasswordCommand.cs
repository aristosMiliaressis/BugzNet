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
using BugzNet.Application.Requests.Identity.Commands;
using System.Text;
using Newtonsoft.Json;
using BugzNet.Core.Utilities;
using BugzNet.Infrastructure.Configuration;

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
        private readonly SignInManager<BugzUser> _signInManager;
        private readonly AppConfig _config;

        public ChangePasswordCommandHandler(BugzNetDataContext context, SignInManager<BugzUser> signInManager, AppConfig config)
        {
            _context = context;
            _signInManager = signInManager;
            _config = config;
        }

        public async Task<CommandResponse> Handle(ChangePasswordCommand command, CancellationToken token)
        {
            BugzUser user = await _signInManager.UserManager.FindByEmailAsync(_signInManager.Context.User.Identity.Name);

            IdentityResult result = await _signInManager.UserManager.ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword);
            if (!result.Succeeded)
            {
                return CommandResponse.WithErrors(result.Errors.Select(e => new CommandError(e.Code, e.Description)).ToArray());
            }

            var cookie = _signInManager.Context.Request.Cookies[AuthState.CookieName];
            var value = cookie.Split(".")[0];
            var state = (AuthState) JsonConvert.DeserializeObject(Encoding.UTF8.GetString(Convert.FromBase64String(value)), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto});

            state = new IdentityVerified();

            var val = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(state, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects})));
            var signature = CryptoUtility.Sign(val, _config.HMACSecret);

            _signInManager.Context.Response.Cookies.Append(AuthState.CookieName, $"{val}.{signature}");

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
