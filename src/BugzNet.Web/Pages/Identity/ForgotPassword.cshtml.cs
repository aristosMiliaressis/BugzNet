using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BugzNet.Infrastructure.Email;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;
using BugzNet.Core.Entities.Identity;

namespace BugzNet.Web.Pages.Identity
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<BugzUser> _userManager;
        private readonly IReportSender _sender;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ForgotPasswordModel(UserManager<BugzUser> userManager, IReportSender sender, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _sender = sender;
            _httpContextAccessor = httpContextAccessor;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist or is not confirmed
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = HttpUtility.UrlEncode(token);

            var context = _httpContextAccessor.HttpContext;
            string callbackUrl = context.Request.Scheme + Uri.SchemeDelimiter + context.Request.Host + context.Request.PathBase + "/Identity/ResetPassword?" + "email=" + Input.Email + "&" + "token=" + token;

            _sender.SendLink("Reset Password", $"Please reset your password by clicking here {callbackUrl}", Input.Email);

            return RedirectToPage("./ForgotPasswordConfirmation");
        }
    }
}
