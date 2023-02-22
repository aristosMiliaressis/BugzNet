using BugzNet.Infrastructure.MediatR;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BugzNet.Application.Requests.Theme.Commands
{
    public class EditSiteThemeCommand : IRequest<CommandResponse>
    {
        public string Theme { get; set; }
    }

    public class EditSiteThemeCommandHandler : IRequestHandler<EditSiteThemeCommand, CommandResponse>
    {
        public async Task<CommandResponse> Handle(EditSiteThemeCommand message, CancellationToken token)
        {
            return CommandResponse.Ok();
        }
    }
}
