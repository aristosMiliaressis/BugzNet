using Audit.Core;
using BugzNet.Core;
using BugzNet.Core.Entities;
using BugzNet.Core.Extensions;
using BugzNet.Core.Localization;
using BugzNet.Core.SharedKernel;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace BugzNet.Infrastructure.DataEF
{
    public static class AuditLogInitializer
    {
        public static void Initialize(IHttpContextAccessor httpContextAccessor)
        {
            Audit.EntityFramework.Configuration.Setup()
                .ForAnyContext(conf => conf.IncludeEntityObjects())
                .UseOptOut()
                .Ignore<AuditLog>();

            Audit.Core.Configuration.AddCustomAction(ActionType.OnScopeCreated, scope =>
            {
                scope.SetCustomField("User", httpContextAccessor?.HttpContext?.User?.Identity?.Name);
                scope.SetCustomField("RequestPath", httpContextAccessor?.HttpContext?.Request?.Path.ToString());
                scope.SetCustomField("TraceIdentifier", httpContextAccessor?.HttpContext?.TraceIdentifier);
                scope.SetCustomField<CancellationToken?>("RequestAborted", httpContextAccessor?.HttpContext?.RequestAborted);
            });

            Audit.Core.Configuration.Setup()
                    .UseEntityFramework(ef => ef
                    .AuditTypeMapper(t => typeof(AuditLog))
                    .AuditEntityAction<AuditLog>((evt, eventEntry, auditEntry) =>
                    {
                        auditEntry.Table = eventEntry.Name;
                        auditEntry.Action = eventEntry.Action == "Insert"
                                             ? AuditAction.Insert
                                             : eventEntry.Action.StartsWith("Update")
                                                ? AuditAction.Update
                                                : AuditAction.Delete;
                        auditEntry.StartDate = LocalizationUtility.UtcToLocal(evt.StartDate);
                        auditEntry.DaysSinceNineteenHundred = (ulong)auditEntry.StartDate.GetDaysSinceNineteenHundred();
                        auditEntry.Username = evt.CustomFields.ContainsKey("User") ? (string)evt.CustomFields["User"] : "__INTERNAL__";
                        auditEntry.RequestPath = evt.CustomFields.ContainsKey("RequestPath") ? (string)evt.CustomFields["RequestPath"] : "";
                        auditEntry.TraceIdentifier = evt.CustomFields.ContainsKey("TraceIdentifier") ? (string)evt.CustomFields["TraceIdentifier"] : "";

                        if (eventEntry.PrimaryKey.Count == 1)
                        {
                            if (eventEntry.PrimaryKey.First().Value is string)
                            {
                                auditEntry.PrimaryKey = (string)eventEntry.PrimaryKey.First().Value;
                            }
                            else
                            {
                                auditEntry.PrimaryKey = eventEntry.PrimaryKey.First().Value.ToString();
                            }
                        }
                        else
                            auditEntry.PrimaryKey = JsonConvert.SerializeObject(eventEntry.PrimaryKey);
                        
                        var domainType = auditEntry.Table.GetEntityType() ?? auditEntry.Table.GetDomainType() ?? auditEntry.Table.GetTypeByString();

                        if (eventEntry.Action == "Delete")
                        {
                            auditEntry.Changes = null;
                        }
                        else if (eventEntry.Action == "Update")
                        {
                            var changes = eventEntry.Changes.Where(c => (c.NewValue == null && c.OriginalValue != null)
                                                                || (c.NewValue != null && !c.NewValue.Equals(c.OriginalValue)));

                            if (!changes.Any())
                                return false;

                            changes = changes.Where(c => domainType.GetProperty(c.ColumnName).GetCustomAttribute(typeof(JsonIgnoreAttribute)) == null);

                            auditEntry.Changes = Convert.ToBase64String(CompressUtility.Zip(JsonConvert.SerializeObject(changes)));
                        }
                        else
                        {        
                            eventEntry.ColumnValues = eventEntry.ColumnValues
                                                                .Where(kv => typeof(ValueObject).IsAssignableFrom(domainType) 
                                                                          || (domainType == null?null:domainType.GetProperty(kv.Key).GetCustomAttribute(typeof(JsonIgnoreAttribute))) == null)
                                                                .ToDictionary(kv => kv.Key, kv => kv.Value);

                            auditEntry.Changes = Convert.ToBase64String(CompressUtility.Zip(JsonConvert.SerializeObject(eventEntry.ColumnValues)));
                        }

                        return true;
                    }).IgnoreMatchedProperties(true)
                );
        }
    }
}
