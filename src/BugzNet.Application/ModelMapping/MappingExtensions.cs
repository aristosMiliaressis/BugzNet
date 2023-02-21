using AutoMapper;
using AutoMapper.QueryableExtensions;

using BugzNet.Application.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace BugzNet.Application.ModelMapping
{
    public static class MappingExtensions
    {
        public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(this IQueryable<TDestination> queryable, int pageNumber, int pageSize)
            => PaginatedList<TDestination>.CreateAsync(queryable, pageNumber, pageSize);

        public static Task<List<TDestination>> ProjectToListAsync<TDestination>(this IQueryable queryable, IConfigurationProvider configuration)
            => queryable.ProjectTo<TDestination>(configuration).ToListAsync();

		public static async Task<PaginatedList<TProjection>> ProjectToPaginatedListAsync<TDestination, TProjection>(this IQueryable<TDestination> source, IConfigurationProvider configuration, int pageNumber, int pageSize, CancellationToken token = default)
		{
			int count = await source.CountAsync(token);

			source = source
						.Skip((pageNumber - 1) * pageSize)
						.Take(pageSize);

			var items = await source
								.ProjectTo<TProjection>(configuration)
								.ToListAsync(token);

			var paginatedList = new PaginatedList<TProjection>(items, count, pageNumber, pageSize);

			return paginatedList;
		}
	}
}
