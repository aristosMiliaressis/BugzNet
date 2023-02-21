using MediatR;
using BugzNet.Core.Extensions;
using BugzNet.Infrastructure.DataEF;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BugzNet.Application.Requests.Audit.Commands
{
    public class DeleteLogsCommand : IRequest<int>
    {
        public DateTime Date { get; set; }
    }

    public class DeleteLogsCommandHandler : IRequestHandler<DeleteLogsCommand, int>
    {
        private readonly BugzNetDataContext _db;

        public DeleteLogsCommandHandler(BugzNetDataContext db)
        {
            _db = db;
        }

        public async Task<int> Handle(DeleteLogsCommand request, CancellationToken cancellationToken)
        {
            var subjectLogs = _db.AuditLog.Where(l => l.DaysSinceNineteenHundred <= request.Date.GetDaysSinceNineteenHundred() 
                                                   && l.Action != Core.Entities.AuditAction.BatchChange);

            int count = subjectLogs.Count();

            _db.AuditLog.RemoveRange(subjectLogs);

            await _db.SaveChangesAsync(cancellationToken);

            return count;
        }
    }
}
