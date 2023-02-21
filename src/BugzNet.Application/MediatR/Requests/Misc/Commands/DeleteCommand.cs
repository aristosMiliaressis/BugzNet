using MediatR;
using BugzNet.Infrastructure.MediatR;
using BugzNet.Infrastructure.DataEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BugzNet.Application.Requests.Misc.Commands
{
    public class DeleteCommand<TEntity> : IRequest<CommandResponse>
    {
        public int Id { get; set; }
    }
    /// <summary>
    /// Generic commad handler for Delete
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    public abstract class DeleteCommandHandler<TEntity> : IRequestHandler<DeleteCommand<TEntity>, CommandResponse>
        where TEntity : class
    {
        private readonly BugzNetDataContext _context;

        public DeleteCommandHandler(BugzNetDataContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Default implementation of the delete handler
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<CommandResponse> Handle(DeleteCommand<TEntity> request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _context.Set<TEntity>().Find(request.Id);
                _context.Remove(entity);
                await _context.SaveChangesAsync(cancellationToken);
                return CommandResponse.OkWithMessage($"{entity.GetType().Name} deleted successfully");
            }
            catch (Exception ex)
            {
                string err = string.Empty;
                if (ex.InnerException != null)
                {
                    err = ex.InnerException.Message.Contains("REFERENCE constraint")
                        ? $"Cannot delete {typeof(TEntity).Name } references to other entities exists.{ex.InnerException.Message.Split(',')[1]}"
                        : string.Empty;
                }

                return CommandResponse.WithError($"An error occurred upon deletion.{err}");
            }
        }
    }
}
