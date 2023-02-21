using AutoMapper;

using BugzNet.Core.Entities;
using BugzNet.Core.Extensions;
using BugzNet.Core.Localization;
using BugzNet.Infrastructure.DataEF;
using BugzNet.Infrastructure.Exporting;
using BugzNet.Infrastructure.Exporting.Models;
using BugzNet.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace BugzNet.Application.Exports
{
    public class AuditLogExport : BaseExport, IExportable
    {
        public AuditLogExport(string fileNameTemp, string extraData = null)
            : base(fileNameTemp, extraData)
        { }

        public override async Task<ExportDto> GenerateAsync(BugzNetDataContext db, IConfigurationProvider config = null)
        {
            ulong daysSinceNineteenHundred = (ulong)LocalizationUtility.LocalTime.AddDays(-1).GetDaysSinceNineteenHundred();
            var auditLogs = await db.AuditLog.Where(l => l.DaysSinceNineteenHundred >= daysSinceNineteenHundred)
                                             .OrderByDescending(l => l.DaysSinceNineteenHundred).ToListAsync();

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                excelPackage.Workbook.Properties.Created = LocalizationUtility.LocalTime;

                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Audit Logs");

                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Cells.AutoFitColumns();

                worksheet.Cells["A1"].Value = "Timestamp";
                worksheet.Cells["B1"].Value = "Actor";
                worksheet.Cells["C1"].Value = "Request Path";
                worksheet.Cells["D1"].Value = "Action";
                worksheet.Cells["E1"].Value = "Object";
                worksheet.Cells["F1"].Value = "Identifier";
                worksheet.Cells["G1"].Value = "Changes";

                int row = 2;
                TypeConverter colorConvertor = TypeDescriptor.GetConverter(Color.Red);
                foreach (var log in auditLogs)
                {
                    worksheet.Cells["A" + row].Value = log.StartDate.ToDateTimeStamp();
                    worksheet.Cells["B" + row].Value = log.Username;
                    worksheet.Cells["C" + row].Value = log.RequestPath;
                    worksheet.Cells["D" + row].Value = log.Action.ToString();

                    Color actionColor = (Color)colorConvertor.ConvertFromString("#FF0000");
                    switch (log.Action)
                    {
                        case AuditAction.Insert:
                            actionColor = (Color)colorConvertor.ConvertFromString("#00e673");
                            break;
                        case AuditAction.Update:
                            actionColor = (Color)colorConvertor.ConvertFromString("#ffad33");
                            break;
                        case AuditAction.Delete:
                            actionColor = (Color)colorConvertor.ConvertFromString("#e60000");
                            break;
                    }
                    worksheet.Cells["D" + row].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells["D" + row].Style.Fill.BackgroundColor.SetColor(actionColor);
               
                    worksheet.Cells["E" + row].Value = log.Table;

                    if (string.IsNullOrEmpty(log.EntityIdentifier))
                    {
                        log.EntityIdentifier = log.PrimaryKey;
                    }

                    worksheet.Cells["F" + row].Value = log.EntityIdentifier;
                    worksheet.Cells["G" + row].Value = log.GetDecodedChanges();

                    row++;
                }
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return new ExportDto()
                {
                    Bytes = excelPackage.GetAsByteArray(),
                    Name = FileNameTemplate.Replace("{DATE_FORMAT}", LocalizationUtility.LocalTime.ToString(DateFormat)),
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                };
            }
        }
    }
}
