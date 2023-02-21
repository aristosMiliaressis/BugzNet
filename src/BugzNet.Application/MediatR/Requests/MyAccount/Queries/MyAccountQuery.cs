using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using BugzNet.Infrastructure.DataEF;
using BugzNet.Application.Requests.MyAccount.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BugzNet.Application.Requests.MyAccount.Queries
{
    public class MyAccountQuery : IRequest<MyAccountProjection>
    {
    }

    public class MyAccountQueryHandler : IRequestHandler<MyAccountQuery, MyAccountProjection>
    {
        private readonly BugzNetDataContext _db;
        private readonly IConfigurationProvider _configuration;
        private readonly string _email;

        public MyAccountQueryHandler(BugzNetDataContext db, IHttpContextAccessor httpContextAccessor, IConfigurationProvider configuration)
        {
            _db = db;
            _configuration = configuration;
            _email = httpContextAccessor.HttpContext.User.Identity.Name;
        }

        public async Task<MyAccountProjection> Handle(MyAccountQuery message, CancellationToken token)
        {
            MyAccountProjection model = await _db.Users
                    .Where(i => i.Email == _email)
                    .ProjectTo<MyAccountProjection>(_configuration)
                    .SingleOrDefaultAsync(token);

            return model;
        }
    }
}
