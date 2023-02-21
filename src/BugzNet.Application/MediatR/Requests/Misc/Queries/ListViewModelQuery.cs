using MediatR;
using BugzNet.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BugzNet.Application.Requests.Misc.Queries
{
    public class ListQuery<TEntity, TViewModel> : IRequest<SearchResult<TViewModel>>
    {
        public string SortOrder { get; set; }
        public string CurrentFilter { get; set; }
        private string _searchString { get; set; }
        public string SearchString { get { return _searchString; } set { _searchString = value?.Trim(); } }
        public int? Page { get; set; }
        public Expression<Func<TEntity, bool>> Predicate { get; set; }
    }
}
