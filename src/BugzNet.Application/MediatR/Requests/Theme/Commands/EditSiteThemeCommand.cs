using BugzNet.Infrastructure.MediatR;
using MediatR;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BugzNet.Application.Requests.Theme.Commands
{
    public class EditSiteThemeCommand : IRequest<CommandResponse>
    {
        public string Theme { get; set; }
        public string Content { get; set; }
    }

    public class EditSiteThemeCommandHandler : IRequestHandler<EditSiteThemeCommand, CommandResponse>
    {
        public async Task<CommandResponse> Handle(EditSiteThemeCommand message, CancellationToken token)
        {
            var fileName = message.Theme;
            var pattern = "\\.\\./";

            while (Regex.Match(fileName, pattern).Success)
                fileName = Regex.Replace(fileName, pattern, "");

            var themePath = Path.Combine($"{AppContext.BaseDirectory}/wwwroot/css/", fileName);

            await File.WriteAllTextAsync(themePath, message.Content, token);

            return CommandResponse.Ok();
        }
    }
}
