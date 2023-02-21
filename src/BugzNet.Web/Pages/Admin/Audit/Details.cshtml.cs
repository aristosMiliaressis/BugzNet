using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using BugzNet.Core.Entities;
using BugzNet.Infrastructure.DataEF;
using BugzNet.Application.Models;
using BugzNet.Application.Requests.Audit.Models;
using BugzNet.Application.Requests.Audit.Queries;
using Newtonsoft.Json;
using BugzNet.Web.Pages.Shared;

namespace BugzNet.Web.Pages.Admin.Audit
{
    public class DetailsModel : PageModelBase
    {
        [BindProperty]
        public DetailedAuditEntryProjection Data { get; set; }

        public async Task OnGetAsync(int id)
        {
            Data = await Mediator.Send(new DetailedAuditEntryQuery() { Id = id });
        }
    }
}
