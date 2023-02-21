using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using BugzNet.Application.Requests.MyAccount.Models;
using BugzNet.Application.Requests.MyAccount.Queries;
using BugzNet.Web.Pages.Shared;

namespace BugzNet.Web.Pages.MyAccount
{
    public class IndexModel : PageModelBase
    {
        [BindProperty]
        public MyAccountProjection Data { get; set; }

        public async Task OnGetAsync(string lastInfoMessage)
        {
            if (!string.IsNullOrEmpty(lastInfoMessage))
                ModelState.AddModelError(nameof(InfoMessage), lastInfoMessage);

            Data = await Mediator.Send(new MyAccountQuery());
        }
    }
}
