using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using BugzNet.Application.ModelMapping;
using BugzNet.Application.Models;
using BugzNet.Infrastructure.DataEF;
using BugzNet.Application.MediatR.Requests.Users.Models;
using BugzNet.Application.Requests.Misc.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BugzNet.Core.Constants;
using BugzNet.Core.Entities.Identity;

namespace BugzNet.Application.MediatR.Requests.Users.Queries
{
    public class ListUsersQueryHandler : IRequestHandler<ListQuery<BugzUser, UserProjection>, SearchResult<UserProjection>>
    {
        private readonly BugzNetDataContext _db;
        private readonly IConfigurationProvider _configuration;

        public ListUsersQueryHandler(BugzNetDataContext db, IConfigurationProvider configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<SearchResult<UserProjection>> Handle(ListQuery<BugzUser, UserProjection> message, CancellationToken token)
        {
            var model = new SearchResult<UserProjection>
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

            IQueryable<BugzUser> BugzUsers = _db.Users
                        .Include(u => u.UserRoles)
                        .ThenInclude(u => u.Role)
                        .Where(u => 1 == 1 && u.UserRoles.All(r => r.Role.Name != BugzRoles.ApiUserRole));

            if (!string.IsNullOrEmpty(message.SearchString))
            {
                BugzUsers = BugzUsers.Where(s => s.UserName.Contains(message.SearchString));
            }
            switch (message.SortOrder)
            {
                case "name_desc":
                    BugzUsers = BugzUsers.OrderByDescending(s => s.UserName);
                    break;
                case "Date":
                    //BugzUsers = BugzUsers.OrderBy(s => s.);
                    break;
                case "date_desc":
                    //BugzUsers = BugzUsers.OrderByDescending(s => s.CreatedOn);
                    break;
                default: // Name ascending 
                    BugzUsers = BugzUsers.OrderBy(s => s.UserName);
                    break;
            }

            int pageSize = PaginatedList<object>.PageSize;
            int pageNumber = (message.Page ?? 1);

            model.Results = await BugzUsers
                .ProjectTo<UserProjection>(_configuration)
                .PaginatedListAsync(pageNumber, pageSize);

            return model;
        }
    }

}
