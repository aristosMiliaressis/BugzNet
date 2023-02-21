using AutoMapper;
using MediatR;
using BugzNet.Application.ModelMapping;
using BugzNet.Core.Entities;
using BugzNet.Core.Localization;
using BugzNet.Infrastructure.DataEF;
using BugzNet.Application.Models;
using BugzNet.Application.Requests.Audit.Models;
using BugzNet.Application.Requests.Misc.Queries;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BugzNet.Core.Extensions;

namespace BugzNet.Application.Requests.Audit.Queries
{
    public class SearchAuditLogsQuery : ListQuery<AuditLog, AuditEntryProjection>
    {
        public string Table { get; set; }
        public string User { get; set; }
        public string Id { get; set; }
        public string Timeframe { get; set; }
        public DateTime? From
        {
            get
            {
                if (string.IsNullOrEmpty(Timeframe) || string.IsNullOrEmpty(Timeframe.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries)[0]))
                    return DateTime.MinValue;

                return DateTime.ParseExact(Timeframe.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries)[0], "dd/MM/yyyy", CultureInfo.InvariantCulture).AddHours(LocalizationUtility.LocalTime.Hour);
            }
        }
        public DateTime To 
        {
            get
            {
                if (string.IsNullOrEmpty(Timeframe))
                    return LocalizationUtility.LocalTime;

                return DateTime.ParseExact(Timeframe.Split(new string[] {" - "}, StringSplitOptions.RemoveEmptyEntries)[1], "dd/MM/yyyy", CultureInfo.InvariantCulture).AddHours(LocalizationUtility.LocalTime.Hour);
            }
        }
    }

    public class SearchAuditLogsQueryHandler : IRequestHandler<SearchAuditLogsQuery, SearchResult<AuditEntryProjection>>
    {
        private readonly BugzNetDataContext _db;
        private readonly IConfigurationProvider _configuration;

        public SearchAuditLogsQueryHandler(BugzNetDataContext db, IConfigurationProvider configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<SearchResult<AuditEntryProjection>> Handle(SearchAuditLogsQuery message, CancellationToken token)
        {
            var model = new SearchResult<AuditEntryProjection>
            {
                CurrentSort = message.SortOrder,
                NameSortParm = string.IsNullOrEmpty(message.SortOrder) ? "name_desc" : "",
                DateSortParm = message.SortOrder == "Date" ? "date_desc" : "Date",
            };

            if (message.SearchString != null)
            {
                message.Page = 1;
            }
            else
            {
                message.SearchString = message.CurrentFilter;
            }

            model.CurrentFilter = message.SearchString;
            model.SearchString = message.SearchString;

            // does not return entries about users to not disclose sensitive info like password hashes
            IQueryable<AuditLog> auditEntries = _db.AuditLog;

            if (!string.IsNullOrEmpty(message.SearchString))
            {
                auditEntries = auditEntries.Where(s => s.Table.Contains(message.SearchString) || s.Username.Contains(message.SearchString));
            }
            if (!string.IsNullOrEmpty(message.Table))
            {
                auditEntries = auditEntries.Where(s => s.Table.Equals(message.Table));
            }
            if (!string.IsNullOrEmpty(message.User))
            {
                auditEntries = auditEntries.Where(s => s.Username.Contains(message.User));
            }
            if (!string.IsNullOrEmpty(message.Id))
            {
                bool isInt = int.TryParse(message.Id, out int intId);
                var primaryKey = JsonConvert.SerializeObject(new object[] { message.Id });
                if (isInt)
                    auditEntries = auditEntries.Where(s => s.PrimaryKey.Equals(message.Id) || s.PrimaryKey == intId.ToString());
                else
                    auditEntries = auditEntries.Where(s => s.PrimaryKey.Equals(message.Id));
            }
            if (!string.IsNullOrEmpty(message.Timeframe))
            {
                auditEntries = auditEntries.Where(s => s.DaysSinceNineteenHundred >= message.From.Value.GetDaysSinceNineteenHundred() 
                                                    && s.DaysSinceNineteenHundred <= message.To.GetDaysSinceNineteenHundred());
            }

            int pageSize = PaginatedList<object>.PageSize;

            int pageNumber = (message.Page ?? 1);

            auditEntries = auditEntries.OrderByDescending(e => e.DaysSinceNineteenHundred);

            model.Results = await auditEntries
                .ProjectToPaginatedListAsync<AuditLog, AuditEntryProjection>(_configuration, pageNumber, pageSize, token);

            return model;
        }
    }
}
