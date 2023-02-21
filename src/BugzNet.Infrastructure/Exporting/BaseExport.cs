using AutoMapper;
using BugzNet.Infrastructure.DataEF;
using BugzNet.Infrastructure.Exporting.Models;
using System.Linq;
using System.Threading.Tasks;

namespace BugzNet.Infrastructure.Exporting
{
    public abstract class BaseExport : IExportable
    {
        protected virtual string DateFormat => "dd_MM_yyyy";
        protected string FileNameTemplate { get; }
        protected string ExtraData { get; set; }

        public BaseExport(string fileNameTemp, string extraData = null)
        {
            FileNameTemplate = fileNameTemp;
            ExtraData = extraData;
        }

        public abstract Task<ExportDto> GenerateAsync(BugzNetDataContext db, IConfigurationProvider config = null);
    }
}
