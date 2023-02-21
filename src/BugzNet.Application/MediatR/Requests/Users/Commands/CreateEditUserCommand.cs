using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using BugzNet.Infrastructure.MediatR;
using BugzNet.Infrastructure.DataEF;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;
using BugzNet.Core.Entities.Identity;
using BugzNet.Core.Constants;

namespace BugzNet.Application.MediatR.Requests.Users.Commands
{
    public class CreateEditUserCommand : IRequest<CommandResponse<string>>
    {
        public string Id { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [IgnoreMap]
        public string Role { get; set; }

        [IgnoreMap]
        public string Password { get; set; }
        [IgnoreMap]
        public string ConfirmationPassword { get; set; }

        [IgnoreMap]
        public SelectList RoleOptions { get; set; }
    }

    public class CreateUserValidator : AbstractValidator<CreateEditUserCommand>
    {
        public CreateUserValidator()
        {
            RuleFor(c => c.Email).NotEmpty().WithMessage("email address is required.");
            RuleFor(c => c.Email)
                .EmailAddress()
                .WithMessage("A valid email address is required.");
            RuleFor(c => c.Role).NotEmpty().WithMessage("role is required.");
            RuleFor(c => c.Password).NotEmpty().WithMessage("password is required.");
            RuleFor(c => c.Password).NotEmpty().MinimumLength(8).WithMessage("password must be at least 8 characters long.");
            RuleFor(c => c.ConfirmationPassword).Equal(c => c.Password).WithMessage("'Confirmation Password' must be equal to 'Password'.");
        }

    }

    public class EditUserValidator : AbstractValidator<CreateEditUserCommand>
    {
        public EditUserValidator()
        {
            RuleFor(c => c.Id).NotEmpty();
            RuleFor(c => c.Email).NotEmpty().WithMessage("email address is required.");
            RuleFor(c => c.Email)
                .EmailAddress()
                .WithMessage("A valid email address is required.");
            RuleFor(c => c.Role).NotEmpty().WithMessage("role is required.");
        }

    }

    public class CreateEditUserCommandHandler : IRequestHandler<CreateEditUserCommand, CommandResponse<string>>
    {
        private readonly BugzNetDataContext _context;
        private readonly UserManager<BugzUser> _userManager;

        public CreateEditUserCommandHandler(BugzNetDataContext context, UserManager<BugzUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<CommandResponse<string>> Handle(CreateEditUserCommand command, CancellationToken token)
        {
            BugzUser BugzUser = null;

            if (command.Role == BugzRoles.ApiUserRole)
                return CommandResponse<string>.WithError("ApiUser role not allowed.");

            if (command.Id == null) //Create
            {
                if (_userManager.FindByEmailAsync(command.Email).Result == null)
                {
                    BugzUser user = new BugzUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = command.Email,
                        Email = command.Email,
                        PhoneNumber = command.PhoneNumber,
                        LockoutEnabled = false,
                        MustChangePassword = true
                    };

                    IdentityResult result = await _userManager.CreateAsync(user, command.Password);
                    if (!result.Succeeded)
                    {
                        return CommandResponse<string>.WithError(user.Id, "Execution affected 0 rows.");
                    }

                    result = await _userManager.AddToRoleAsync(user, command.Role);
                    if (!result.Succeeded)
                    {
                        return CommandResponse<string>.WithError(user.Id, $"Failed to add user to role {command.Role}");
                    }

                    return CommandResponse<string>.OkWithMessage(user.Id, $"User {user.Email} with role {command.Role} Created.");
                }

                return CommandResponse<string>.WithError(command.Id, $"User {command.Email} already exists.");
            }
            else // Update
            {
                BugzUser = _context.Users.Find(command.Id);
                BugzUser.UserName = command.Email;
                BugzUser.NormalizedUserName = command.Email.ToUpper();
                BugzUser.Email = command.Email;
                BugzUser.NormalizedEmail = command.Email.ToUpper();
                BugzUser.PhoneNumber = command.PhoneNumber;
                var result = await _userManager.UpdateAsync(BugzUser);
                if (!result.Succeeded)
                {
                    return CommandResponse<string>.WithError(command.Id, "Failed updating users data.");
                }

                var userRole = _context.UserRoles.Where(ur => ur.UserId == BugzUser.Id).First();
                var previousRole = _context.Roles.Find(userRole.RoleId);
                var newRole = _context.Roles.Where(r => r.Name == command.Role).FirstOrDefault();
                if (newRole == null)
                {
                    return CommandResponse<string>.WithError("Failed changing users role.");
                }

                if (previousRole.Name != newRole.Name)
                {
                    result = await _userManager.RemoveFromRoleAsync(BugzUser, previousRole.Name);
                    if (!result.Succeeded)
                    {
                        return CommandResponse<string>.WithError(command.Id, "Failed changing users role.");
                    }
                    result = await _userManager.AddToRoleAsync(BugzUser, newRole.Name);
                    if (!result.Succeeded)
                    {
                        return CommandResponse<string>.WithError(command.Id, $"Role {previousRole.Name} removed from user, but role {command.Role} wasn't added.");
                    }
                }

                await _context.SaveChangesAsync(token);

                return CommandResponse<string>.OkWithMessage(command.Id, $"User {BugzUser.Email} Updated.");
            }
        }
    }
}
