using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;

using BugzNet.Core.Entities;
using BugzNet.Infrastructure.DataEF;
using BugzNet.Application.Models;
using BugzNet.Application.Requests.Audit.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace BugzNet.Application.Requests.Audit.Queries
{
    public class DetailedAuditEntryQuery : IRequest<DetailedAuditEntryProjection>
    {
        public int Id { get; set; }
    }

    public class GetQueryHandler : IRequestHandler<DetailedAuditEntryQuery, DetailedAuditEntryProjection>
    {
        private readonly BugzNetDataContext _db;
        private readonly IConfigurationProvider _configuration;

        public GetQueryHandler(BugzNetDataContext db, IConfigurationProvider configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<DetailedAuditEntryProjection> Handle(DetailedAuditEntryQuery message, CancellationToken token)
        {
            DetailedAuditEntryProjection model = await _db.AuditLog
                    .Where(a => a.Id == message.Id)
                    .ProjectTo<DetailedAuditEntryProjection>(_configuration)
                    .SingleOrDefaultAsync(token);

            return model;
        }
    }
}
