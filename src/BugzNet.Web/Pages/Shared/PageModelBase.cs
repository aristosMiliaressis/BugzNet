using MediatR;
using Microsoft.Extensions.DependencyInjection;
using BugzNet.Infrastructure.MediatR;
using BugzNet.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace BugzNet.Web.Pages.Shared
{
    public abstract class PageModelBase : PageModel
    {
        [BindProperty]
        public string InfoMessage { get; set; }

        protected IMediator Mediator => HttpContext.RequestServices.GetService<IMediator>();

        public virtual void HandleDisplayMessages(CommandResponse commandResponse) 
        {
            HandleDisplayMessages(commandResponse.Message, commandResponse.ErrorsAsString);
        }

        protected void HandleDisplayMessages(string lastInfoMessage, string lastErrorMessage)
        {
            if (lastInfoMessage.Length > 0)
                ModelState.AddModelError(nameof(InfoMessage), lastInfoMessage);
            else if (!string.IsNullOrEmpty(lastErrorMessage))
                ModelState.AddErrorMessages(lastErrorMessage);
        }
    }
}
