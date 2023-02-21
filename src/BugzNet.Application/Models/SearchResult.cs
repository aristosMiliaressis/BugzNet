using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugzNet.Application.Models
{
    public class SearchResult<TViewModel>
    {
        public string CurrentSort { get; set; }
        public string NameSortParm { get; set; }
        public string DateSortParm { get; set; }
        public string CurrentFilter { get; set; }
        public string SearchString { get; set; }
        public bool AllowCreate { get; set; } = true;
        public PaginatedList<TViewModel> Results { get; set; }
    }
}
