using BugzNet.Application.Requests.Theme.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BugzNet.Application.Requests.Theme.Queries
{
    public class GetSiteThemeQuery : IRequest<EditSiteThemeCommand>
    {
        public string Theme { get; set; }
    }

    public class GetSiteThemeQueryHandler : IRequestHandler<GetSiteThemeQuery, EditSiteThemeCommand>
    {
        public async Task<EditSiteThemeCommand> Handle(GetSiteThemeQuery message, CancellationToken token)
        {
            return null;
        }
    }
}
