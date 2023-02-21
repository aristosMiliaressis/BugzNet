using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BugzNet.Core.Entities.Identity;

namespace BugzNet.Application.Requests.Identity.Commands
{
    public class LogoutCommand : IRequest<bool> { }
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
    {
        private readonly SignInManager<BugzUser> _signInManager;

        public LogoutCommandHandler(SignInManager<BugzUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<bool> Handle(LogoutCommand message, CancellationToken token)
        {
            await _signInManager.SignOutAsync();

            return true;
        }
    }
}
