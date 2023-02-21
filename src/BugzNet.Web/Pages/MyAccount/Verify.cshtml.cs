using Microsoft.AspNetCore.Authorization;
using BugzNet.Web.Pages.Shared;
using BugzNet.Core.Utilities;
using BugzNet.Infrastructure.SMS;
using System.Threading.Tasks;
using BugzNet.Infrastructure.DataEF;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using BugzNet.Application.Requests.MyAccount.Queries;
using BugzNet.Application.Requests.MyAccount.Commands;

namespace BugzNet.Web.Pages.Identity
{
    public class Verify : PageModelBase
    {
        private ISmsSender _smsSender;
        private BugzNetDataContext _context;

        public Verify(ISmsSender smsSender, BugzNetDataContext context)
        {
            _smsSender = smsSender;
            _context = context;
        }

        [BindProperty]
        public OtpVerificationQuery Data { get; set; }

        public void OnGet(string lastErrorMessage = "")
        {
            HandleDisplayMessages("", lastErrorMessage);

        }

        public async Task<IActionResult> OnPostVerifyAsync([FromQuery] string returnUrl)
        {
            Data.UserEmail = HttpContext.User.Identity.Name;

            var verified = await Mediator.Send(Data);
            if (!verified)
            {
                return RedirectToPage(nameof(Verify), new { lastErrorMessage = "Invalid OTP Code."});
            }

            // TODO: handle verification result (add/remove claim)
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToPage("/Bugs/Index");
        }

        public async Task OnPostGenerateAsync()
        {
            var command = new GenerateOtpCommand
            {
                UserEmail = HttpContext.User.Identity.Name
            };

            await Mediator.Send(command);
        }
    }
}
