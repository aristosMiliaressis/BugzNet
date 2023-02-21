using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BugzNet.Web.Pages.Bugs
{
    [AllowAnonymous]
    public class Error : PageModel
    {
        public string ErrorMessage { get; set; }
        public int ErrorCode { get; set; }

        public void OnGet(int code)
        {
            HandleErrorCode(code);
        }

        public void OnPost(int code)
        {
            HandleErrorCode(code);
        }

        private void HandleErrorCode(int code)
        {
            ErrorCode = code;
            ErrorMessage = code switch
            {
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Page Not Found",
                408 => "Request Timed out",
                >= 400 and < 500 => "Client Error",
                >= 500 => "Server Error",
                _ => "Unknown Error"
            };
        }
    }
}
