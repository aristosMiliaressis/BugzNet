using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BugzNet.Web.Pages.Shared;
using BugzNet.Application.Requests.Theme.Commands;
using BugzNet.Application.Requests.Theme.Queries;

namespace BugzNet.Web.Pages.Admin.Theme
{
    public class Edit : PageModelBase
    {
        [BindProperty]
        public EditSiteThemeCommand Data { get; set; }

        public async Task OnGetAsync(string id, string lastInfoMessage = "", string lastErrorMessage = "")
        {
            HandleDisplayMessages(lastInfoMessage, lastErrorMessage);

            Data = await Mediator.Send(new GetSiteThemeQuery() { Theme = id });
        }

        public async Task<IActionResult> OnPostAsync()
        {
             var commandResponse = await Mediator.Send(Data);

            if (commandResponse.IsSuccess)
            {
                return RedirectToPage(nameof(Edit), new {lastInfoMessage = commandResponse.Message });
            }
            else
            {
                return RedirectToPage(nameof(Edit), new { lastErrorMessage = commandResponse.ErrorsAsString });
            }
        }
    }
}
