using BugzNet.Application.Models;
using BugzNet.Web.Pages.Shared;
using System.Collections.Generic;

namespace BugzNet.Web.Pages.Admin.Theme
{
    public class Index : PageModelBase
    {
        public SearchResult<ThemeProjection> Data { get; protected set; }

        public void OnGet(string sortOrder, string currentFilter, string searchString, int? pageIndex, string lastErrorMessage = "", string lastInfoMessage = "")
        {
            HandleDisplayMessages(lastInfoMessage, lastErrorMessage);

            Data = new SearchResult<ThemeProjection>
            {
                Results = new PaginatedList<ThemeProjection>(new List<ThemeProjection> { new ThemeProjection { Name = "custom.css" }}, 1, 1, 20)
            };
        }
    }

    public class ThemeProjection
    {
        public string Name { get; set; }
    }
}
