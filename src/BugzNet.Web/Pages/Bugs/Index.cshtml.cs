using System.Collections.Generic;
using System.Threading.Tasks;
using BugzNet.Application.MediatR.Requests.Users.Queries;
using BugzNet.Core.ValueObjects;
using BugzNet.Infrastructure.DataJson;
using BugzNet.Infrastructure.DataJson.Interfaces;
using BugzNet.Web.Pages.Shared;

namespace BugzNet.Web.Pages.Bugs
{
    public class Index : PageModelBase
    {
        private readonly IValueObjectRepository<Bug> _repo;

        public Index(IValueObjectRepository<Bug> repo)
        {
            _repo = repo;
        }

        public List<Bug> Bugs { get; private set; }

        public void OnGet()
            => Bugs = _repo.List();
    }
}
