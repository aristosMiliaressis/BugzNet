using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using BugzNet.Application.Models;
using BugzNet.Application.Requests.Misc.Queries;
using BugzNet.Web.Pages.Shared;
using BugzNet.Application.MediatR.Requests.Users.Commands;
using BugzNet.Application.MediatR.Requests.Users.Models;
using BugzNet.Core.Entities.Identity;

namespace BugzNet.Web.Pages.Admin.Users
{
    public class Index : PageModelBase
    {
        public SearchResult<UserProjection> Data { get; protected set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex, string lastErrorMessage = "", string lastInfoMessage = "")
        {
            HandleDisplayMessages(lastInfoMessage, lastErrorMessage);

            Data = await Mediator.Send(new ListQuery<BugzUser, UserProjection>
            {
                CurrentFilter = currentFilter,
                Page = pageIndex,
                SearchString = searchString,
                SortOrder = sortOrder
            });
        }

        public async Task<IActionResult> OnGetDelete(string id, string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            var commandResponse = await Mediator.Send(new DeleteUserCommand() { Id = id });

            return RedirectToPage(nameof(Index),
             new
             {
                 sortOrder,
                 currentFilter,
                 searchString,
                 pageIndex,
                 lastErrorMessage = commandResponse.ErrorsAsString,
                 lastInfoMessage = commandResponse.Message
             });
        }
    }
}
