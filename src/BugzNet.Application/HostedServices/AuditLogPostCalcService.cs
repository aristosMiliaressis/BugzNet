
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;
// using BugzNet.Infrastructure.Configuration;
// using BugzNet.Infrastructure.Configuration.Models;
// using BugzNet.Infrastructure.DataEF;
// using BugzNet.Infrastructure.Extensions;
// using BugzNet.Infrastructure.HostedServices;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Linq.Dynamic.Core;
// using System.Threading;
// using System.Threading.Tasks;

// namespace BugzNet.Application.HostedServices
// {
//     public class AuditLogPostCalcService : BaseHostedService
//     {
//         private readonly IServiceScopeFactory _serviceScopeFactory;

//         public AuditLogPostCalcService(IServiceScopeFactory serviceScopeFactory, AppConfig config)
//             : base(config.Workers)
//         {
//             _serviceScopeFactory = serviceScopeFactory;
//         }

//         protected override async Task RunAsync(CancellationToken token)
//         {
//             try
//             {
//                 using (var scope = _serviceScopeFactory.CreateScope())
//                 {
//                     var dbContext = scope.ServiceProvider.GetService<BugzNetDataContext>();

//                     while (true)
//                     {

//                         var entry = await dbContext.AuditLog
//                                             .Where(l => l.EntityIdentifier == null)
//                                             .FirstOrDefaultAsync(token);
//                         if (entry == null)
//                         {
//                             _logger.LogDebug("{ServiceName} stoped because no unprocessed entries found.", ServiceName);
//                             return;
//                         }

//                         try
//                         {
//                             entry.EntityIdentifier = await dbContext.ConstructRelationshipIdentifierFromDependentEndRecursivly(entry.Table.GetEntityType(), entry.PrimaryKey);
//                         }
//                         catch (Exception ex) when (ex is not OperationCanceledException)
//                         {
//                             _logger.LogException(ex);
//                         }

//                         if (entry.EntityIdentifier == null)
//                             entry.EntityIdentifier = entry.PrimaryKey;

//                         await dbContext.TrySaveChangesAsync(token);
//                     }
//                 }
//             }
//             catch (Exception ex) when (ex is not OperationCanceledException)
//             {
//                 _logger.LogException(ex, critical: false);
//             }
//         }
//     }
// }
