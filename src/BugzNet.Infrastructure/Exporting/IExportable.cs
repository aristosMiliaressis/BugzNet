using AutoMapper;
using BugzNet.Infrastructure.DataEF;
using BugzNet.Infrastructure.Exporting.Models;
using System.Threading.Tasks;

namespace BugzNet.Infrastructure.Exporting
{
    public interface IExportable
    {
        Task<ExportDto> GenerateAsync(BugzNetDataContext db, IConfigurationProvider config = null);
    }
}
