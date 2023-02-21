using MediatR;

using BugzNet.Infrastructure.MediatR;
using BugzNet.Infrastructure.DataEF;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BugzNet.Core.Constants;

namespace BugzNet.Application.MediatR.Requests.Users.Commands
{
    public class DeleteUserCommand : IRequest<CommandResponse>
    {
        public string Id { get; set; }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, CommandResponse>
    {
        private readonly BugzNetDataContext _context;

        public DeleteUserCommandHandler(BugzNetDataContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _context.Users
                                    .Include(u => u.UserRoles)
                                    .ThenInclude(u => u.Role)
                                    .FirstOrDefault(u => u.Id == request.Id);

                if (entity == null)
                    return CommandResponse.WithError($"user not found");

                if (entity.UserRoles.FirstOrDefault(e => e.Role.Name == BugzRoles.ApiUserRole) != null)
                    return CommandResponse.WithError($"Could not delete user");

                _context.Remove(entity);
                await _context.SaveChangesAsync(cancellationToken);
                return CommandResponse.OkWithMessage($"{entity.Email} deleted successfully");
            }
            catch (Exception ex)
            {
                string err = string.Empty;
                if (ex.InnerException != null)
                {
                    err = ex.InnerException.Message.Contains("REFERENCE constraint")
                        ? $"Cannot delete user references to other entities exists.{ex.InnerException.Message.Split(',')[1]}"
                        : string.Empty;
                }

                return CommandResponse.WithError($"An error occurred upon deletion.{err}");
            }
        }
    }
}
