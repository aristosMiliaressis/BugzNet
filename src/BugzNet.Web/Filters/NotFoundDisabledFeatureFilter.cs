using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using System;
using System.Threading.Tasks;

namespace BugzNet.Web.Filters
{
    public class NotFoundDisabledFeatureFilterFactory : Attribute, IFilterFactory
    {
        public string Feature { get; set; }
        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var featureManager = serviceProvider.GetService<IFeatureManager>();

            var filter = new NotFoundDisabledFeatureFilter(featureManager);
            filter.Feature = Feature;

            return filter;
        }
    }

    public class NotFoundDisabledFeatureFilter : IAsyncPageFilter
    {
        private readonly IFeatureManager _featureManager;
        public string Feature { get; set; }

        public NotFoundDisabledFeatureFilter(IFeatureManager featureManager)
        {
            _featureManager = featureManager;
        }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            if (!await _featureManager.IsEnabledAsync(Feature))
            {
                context.Result = new NotFoundResult();
                return;
            }

            await next.Invoke();
        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }
    }
}
