using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using BugzNet.Core.Entities;
using BugzNet.Application.Models;
using BugzNet.Application.Requests.Audit.Commands;
using BugzNet.Application.Requests.Audit.Models;
using BugzNet.Application.Requests.Audit.Queries;
using BugzNet.Web.Pages.Shared;
using System.Threading;
using BugzNet.Application.Exports;
using BugzNet.Infrastructure.DataEF;

namespace BugzNet.Web.Pages.Admin.Audit
{
    public class Index : PageModelBase
    { 
        public SearchResult<AuditEntryProjection> Data { get; set; }

        [BindProperty(SupportsGet = true)]
        public SearchAuditLogsQuery Query { get; set; }
        private readonly BugzNetDataContext _db;

        public Index(BugzNetDataContext db)
        {
            _db = db;
        }

        public async Task OnGetAsync(string sortOrder, int? pageIndex, string lastErrorMessage = "", string lastInfoMessage = "")
        {
            HandleDisplayMessages(lastInfoMessage, lastErrorMessage);

            Query.SortOrder = sortOrder;
            Query.Page = pageIndex;

            Data = await Mediator.Send(Query, HttpContext.RequestAborted);
        }

        public async Task<IActionResult> OnGetExport()
        {
            var report = new AuditLogExport("BugzNet_AUDITLOG_{DATE_FORMAT}.xlsx");
            var export = await report.GenerateAsync(_db);

            return File(export.Bytes, export.ContentType, export.Name);
        }

        public async Task<IActionResult> OnGetDelete(DateTime date)
        {
            var deletedEntries = await Mediator.Send(new DeleteLogsCommand { Date = date });

            return RedirectToPage(nameof(Index),
                new { lastInfoMessage = $"{deletedEntries} audit entries recorded prior to {date.ToShortDateString()} were deleted." });
        }
    }
}
