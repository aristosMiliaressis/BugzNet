using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using BugzNet.Application.Requests.Identity.Commands;
using Microsoft.AspNetCore.Authorization;
using BugzNet.Web.Pages.Shared;

namespace BugzNet.Web.Pages.Identity
{
    [AllowAnonymous]
    public class Login : PageModelBase
    {
        [BindProperty]
        public LoginCommand Data { get; set; }

        #region Page Handlers

        public IActionResult OnGet(string returnUrl, [FromQuery] string rememberMe)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Bugs/Index");
            }

            Data = new LoginCommand();
            ViewData["rememberMe"] = rememberMe ?? "false";
            ViewData["returnUrl"] = returnUrl;

            return Page();
        }
    
        public async Task<IActionResult> OnPostLoginAsync([FromQuery] string returnUrl)
        {
            bool signInSuccess =  await Mediator.Send(Data);
            if(!signInSuccess)
            {
                if (ModelState.ErrorCount == 0)
                    ModelState.AddModelError("", "Invalid Login specified!");
                return Page();
            }
            else
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToPage("/Bugs/Index");
            }
        }

        public async Task<IActionResult> OnGetLogoutAsync()
        {
            bool signOutSuccess = await Mediator.Send(new LogoutCommand());

            return RedirectToPage(nameof(Login));
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            await Mediator.Send(Data);

            return RedirectToPage(nameof(Login));
        }
        #endregion
    }
}
