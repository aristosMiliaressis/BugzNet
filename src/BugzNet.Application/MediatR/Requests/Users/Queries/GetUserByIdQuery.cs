using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;

using BugzNet.Infrastructure.DataEF;
using BugzNet.Application.MediatR.Requests.Users.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BugzNet.Core.Entities.Identity;
using BugzNet.Core.Constants;

namespace BugzNet.Application.MediatR.Requests.Users.Queries
{
    public class GetUserByIdQuery : IRequest<CreateEditUserCommand>
    {
        public string Id { get; set; }
    }

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, CreateEditUserCommand>
    {
        private readonly BugzNetDataContext _db;
        private readonly IConfigurationProvider _configuration;

        public GetUserByIdQueryHandler(BugzNetDataContext db, IConfigurationProvider configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<CreateEditUserCommand> Handle(GetUserByIdQuery message, CancellationToken token)
        {
            var model = new CreateEditUserCommand();
            if (!string.IsNullOrEmpty(message.Id))
            {
                model = await _db.Users
                        .Where(i => i.Id == message.Id)
                        .ProjectTo<CreateEditUserCommand>(_configuration)
                        .SingleOrDefaultAsync(token);

                model.Role = _db.UserRoles
                                    .Include(ur => ur.Role)
                                    .Where(ur => ur.UserId == message.Id)
                                    .First().Role.Name;
            }

            model.RoleOptions = new SelectList(_db.Roles.Where(r => r.Name != BugzRoles.ApiUserRole),
                   nameof(BugzRole.Name),
                   nameof(BugzRole.Name));
            return model;
        }
    }
}
