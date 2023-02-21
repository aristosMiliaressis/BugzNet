using Microsoft.AspNetCore.Mvc;
using BugzNet.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugzNet.Web.Pages.Shared
{
    public abstract class IndexPageModel<TEntity, TViewModel> : PageModelBase
        where TEntity : class
        where TViewModel : class
    {
        public SearchResult<TViewModel> Data { get; protected set; }
        public abstract Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex, string lastErrorMessage = "", string lastInfoMessage = "");
        public abstract Task<IActionResult> OnGetDelete(int id, string sortOrder, string currentFilter, string searchString, int? pageIndex);
    }
}
