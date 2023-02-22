using BugzNet.Application.Requests.Theme.Commands;
using MediatR;
using System;
using System.IO;
using System.Text.RegularExpressions;
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
            var fileName = message.Theme;
            var pattern = "\\.\\./";

            while (Regex.Match(fileName, pattern).Success)
                fileName = Regex.Replace(fileName, pattern, "");

            var themePath = Path.Combine($"{AppContext.BaseDirectory}/wwwroot/css/", fileName);

            return new EditSiteThemeCommand
            {
                Theme = message.Theme,
                Content = await File.ReadAllTextAsync(themePath, token)
            };
        }
    }
}
