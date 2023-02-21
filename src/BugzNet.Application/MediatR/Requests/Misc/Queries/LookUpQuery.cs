using AutoMapper;
using MediatR;

using BugzNet.Core.SharedKernel;
using BugzNet.Infrastructure.DataEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BugzNet.Application.MediatR.Requests.Misc.Queries
{
    public class LookUpQuery<T> : IRequest<IEnumerable<T>> where T : BaseEntity<int>
    {
    }

    public class LookUpQueryHandler<T> : IRequestHandler<LookUpQuery<T>, IEnumerable<T>> where T : BaseEntity<int>
    {
        private readonly BugzNetDataContext _db;

        public LookUpQueryHandler(BugzNetDataContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<T>> Handle(LookUpQuery<T> message, CancellationToken token)
        {
            return await _db.Set<T>().ToListAsync(token);
        }
    }
}
