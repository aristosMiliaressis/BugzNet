using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BugzNet.Web.Pages.Shared;
using BugzNet.Application.MediatR.Requests.Users.Commands;
using BugzNet.Application.MediatR.Requests.Users.Queries;
using BugzNet.Web.Extensions;

namespace BugzNet.Web.Pages.Admin.Users
{
    public class CreateEdit : PageModelBase
    {
        [BindProperty]
        public CreateEditUserCommand Data { get; set; }

        #region Page Handlers
        public async Task OnGetAsync(string id, string lastInfoMessage = "", string lastErrorMessage = "")
        {
            HandleDisplayMessages(lastInfoMessage, lastErrorMessage);

            Data = await Mediator.Send(new GetUserByIdQuery() { Id = id });
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!await IsValidForAsync("create"))
            {
                return RedirectToPage(nameof(CreateEdit), new { lastErrorMessage = ModelState.ToErrorString() });
            }

            var commandResponse = await Mediator.Send(Data);

            if (commandResponse.IsSuccess)
            {
                return RedirectToPage(nameof(Index), new { lastInfoMessage = commandResponse.Message });
            }
            else
            {
                return RedirectToPage(nameof(CreateEdit), new { id = Data.Id, lastErrorMessage = commandResponse.ErrorsAsString });
            }
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (!await IsValidForAsync("edit"))
            {
                return RedirectToPage(nameof(CreateEdit), new { id = Data.Id, lastErrorMessage = ModelState.ToErrorString() });
            }

            var commandResponse = await Mediator.Send(Data);

            if (commandResponse.IsSuccess)
            {
                return RedirectToPage(nameof(CreateEdit), new { id = Data.Id, lastInfoMessage = commandResponse.Message });
            }
            else
            {
                return RedirectToPage(nameof(CreateEdit), new { id = Data.Id, lastErrorMessage = commandResponse.ErrorsAsString });
            }
        }

        #endregion

        public async Task<bool> IsValidForAsync(string action)
        {
            FluentValidation.Results.ValidationResult validationRes;
            if (action == "create")
            {
                var validator = new CreateUserValidator();
                validationRes = validator.Validate(Data);

            }
            else
            {
                var validator = new EditUserValidator();
                validationRes = validator.Validate(Data);
            }

            if (!validationRes.IsValid)
            {
                Data = await Mediator.Send(new GetUserByIdQuery() { Id = Data.Id });
                foreach (var error in validationRes.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                return false;
            }
            ModelState.Clear();
            return true;
        }
    }
}
