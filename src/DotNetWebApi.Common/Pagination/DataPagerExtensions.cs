using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DotNetWebApi.Common.Pagination
{
    public static class DataPagerExtensions
    {
        public static async Task<PagedModel<TModel>> PaginateAsync<TModel>(
            this IQueryable<TModel> query,
            int limit,
            int page,
            CancellationToken cancellationToken)
            where TModel : class
        {

            var paged = new PagedModel<TModel>();

            page = (page < 0) ? 1 : page;

            paged.CurrentPage = page;
            paged.PageSize = limit;

            var totalItemsCountTask = query.CountAsync(cancellationToken);

            var startRow = (page - 1) * limit;
            paged.Items = await query
                .Skip(startRow)
                .Take(limit)
                .ToListAsync(cancellationToken);

            paged.TotalItems = await totalItemsCountTask;
            paged.TotalPages = (int)Math.Ceiling(paged.TotalItems / (double)limit);

            return paged;
        }

        public static void AddResourceLink(this ILinkedResource resources,
            LinkedResourceType resourceType,
            string routeUrl)
        {
            resources.Links ??= new Dictionary<LinkedResourceType, string>();
            resources.Links.Add(resourceType, routeUrl);
        }
    }
}
