using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugzNet.Application.Models
{
    public interface ISelectable
    {
        int Id { get; set; }
        int TemplateId { get; set; }
        string DisplayName { get; set; }
        string BindName { get; set; }
        bool IsSelected { get; set; }
        bool IsDisabled { get; set; }
        bool CanBeDeleted { get; set; }
        bool AllowEdit { get; }
    }
}
