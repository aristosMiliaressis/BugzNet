using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BugzNet.Application.Requests.MyAccount.Commands;
using BugzNet.Web.Pages.Shared;

namespace BugzNet.Web.Pages.MyAccount
{
    public class ChangePasswordModel : PageModelBase
    {
        [BindProperty]
        public ChangePasswordCommand Data { get; set; }

        #region Page Handlers
        public void OnGet()
        {
            return;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            { // re-render the view when validation failed.
                return Page();
            }

            var commandResponse = await Mediator.Send(Data);

            HandleDisplayMessages(commandResponse);

            if (commandResponse.IsSuccess)
            {
                return Redirect($"{Request.PathBase}/Identity/Login");
            }
            else
            {
                return Page();
            }
        }
        #endregion
    }
}
