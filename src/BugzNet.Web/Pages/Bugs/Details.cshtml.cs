using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using BugzNet.Core.ValueObjects;
using BugzNet.Infrastructure.DataJson;
using BugzNet.Infrastructure.DataJson.Interfaces;
using BugzNet.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BugzNet.Web.Pages.Bugs
{
    public class Details : PageModelBase
    {
        private readonly IValueObjectRepository<Bug> _repo;

        public Details(IValueObjectRepository<Bug> repo)
        {
            _repo = repo;
        }

        [BindProperty]
        public Bug Bug { get; set; }

        public void OnGet([FromQuery] string name)
        {
            Bug = _repo.List().FirstOrDefault(b => b.Name == name);
            if (Bug == null)
            {
                Redirect("/Bugs/Error?code=404");
            }
        }

        public IActionResult OnPost()
        {
            var bugs = _repo.List();

            var bug = bugs.FirstOrDefault(b => b.Name == Bug.Name);
            bugs.Remove(bug);

            bug.Description = Bug.Description;
            bug.CanFly = Bug.CanFly;
            bug.Bites = Bug.Bites;
            bugs.Add(bug);

            var json = JsonConvert.SerializeObject(bugs);
            var filePath = Path.Combine($"{AppContext.BaseDirectory}/App_Data", "bugs.json");
            
            System.IO.File.WriteAllText(filePath, json);

            return RedirectToPage(nameof(Details), new { name = bug.Name });
        }
    }
}
