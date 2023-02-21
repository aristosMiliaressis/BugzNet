using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BugzNet.Core.Entities;
using BugzNet.Core.Localization;
using BugzNet.Infrastructure.Configuration;
using BugzNet.Infrastructure.DataEF;
using BugzNet.Infrastructure.Email;
using BugzNet.Infrastructure.Exporting;
using BugzNet.Infrastructure.Exporting.Models;
using BugzNet.Infrastructure.Extensions;
using BugzNet.Infrastructure.HostedServices;
using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace BugzNet.Application.HostedServices
{
    public class ReportingService : BaseHostedService
    {
        private readonly IConfigurationProvider _mapperConfig;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly List<ReportSettings> _reports;
        private readonly IReportSender _sender;
        private readonly AppConfig _config;

        public ReportingService(IServiceScopeFactory serviceScopeFactory, 
            AppConfig config, IReportSender sender, IConfigurationProvider mapperConfig)
            : base(config.Workers)
        {
            _reports = config.Reports;
            _sender = sender;
            _config = config;
            _mapperConfig = mapperConfig;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            var iface = typeof(IExportable);
            var exportTypes = AppDomain.CurrentDomain.GetAssemblies()
                                    .SelectMany(a => a.GetTypes())
                                    .Where(t => iface.IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var report in _reports)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    CrontabSchedule schedule;
                    try
                    {
                        schedule = CrontabSchedule.Parse(report.CronExpression);
                    }
                    catch (CrontabException cronEx)
                    {
                        _logger.LogCritical("Failed to parse {report}s cron Expression:\r\n{message}", report.Name, cronEx.Message);
                        continue;
                    }

                    var dbContext = scope.ServiceProvider.GetService<BugzNetDataContext>();

                    var lastSuccessfullSendOp = dbContext.ReportingOperations.FirstOrDefault(o => o.Name == report.Name);
                    var lastSuccessfulSendOp = lastSuccessfullSendOp?.LastOccurrence ?? LocalizationUtility.LocalTime;
                    var nextScheduledSendOp = schedule.GetNextOccurrence(lastSuccessfulSendOp);

                    if (lastSuccessfullSendOp == null)
                    {
                        lastSuccessfullSendOp = new ReportingOperation(report.Name);
                        lastSuccessfullSendOp.SetNextOccurrence(nextScheduledSendOp);
                        dbContext.ReportingOperations.Add(lastSuccessfullSendOp);
                        await dbContext.SaveChangesAsync(token);
                    }

                    if (LocalizationUtility.LocalTime >= lastSuccessfullSendOp.NextOccurrence)
                    {
                        _logger.LogInformation("Processing Scheduled Report {report}...", report.Name);

                        var exports = new List<ExportDto>();
                        foreach (var exportSettings in report.Exports)
                        {
                            var type = exportTypes.FirstOrDefault(t => t.Name == exportSettings.Name);
                            if (type == null)
                            {
                                _logger.LogCritical("export {export} not found.", exportSettings.Name);
                                continue;
                            }

                            object instance = Activator.CreateInstance(type, new object[] { exportSettings.FileNameTemplate, exportSettings.ExtraData });

                            MethodInfo method = type.GetMethod(nameof(IExportable.GenerateAsync));

                            ExportDto export;
                            try
                            {
                                export = await (Task<ExportDto>)method.Invoke(instance, new object[2] { dbContext, _config });
                            }
                            catch (Exception ex)
                            {
                                _logger.LogCritical("Error while generating export '{export}'", type.Name);
                                _logger.LogException(ex);
                                continue;
                            }

                            exports.Add(export);
                        }

                        try
                        {
                            _sender.Send(exports.ToArray(), report.Subject.Replace("{DATE_FORMAT}", LocalizationUtility.LocalTime.ToString("dd.MM.yyyy")), report.Body, report.Recipients);
                            _logger.LogInformation("{report} Sent with {sender}", report.Name, _sender.GetType().Name);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogCritical("Error while sending report '{report}'", report.Name);
                            _logger.LogException(ex);
                            continue;
                        }

                        lastSuccessfullSendOp.SetLastOccurrence(LocalizationUtility.LocalTime);
                        lastSuccessfullSendOp.SetNextOccurrence(schedule.GetNextOccurrence(LocalizationUtility.LocalTime));
                        dbContext.ReportingOperations.Update(lastSuccessfullSendOp);
                        await dbContext.SaveChangesAsync(token);
                    }
                }
            }
        }
    }
}
