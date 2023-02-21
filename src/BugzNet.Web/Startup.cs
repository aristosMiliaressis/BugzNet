using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BugzNet.Web.Extensions;
using BugzNet.Infrastructure.Configuration;
using BugzNet.Core.Localization;
using BugzNet.Web.Middleware;
using System.Threading.Tasks;
using Microsoft.AspNetCore.HttpOverrides;

namespace BugzNet.Web
{
    public class Startup
    {
        private readonly AppConfig _config;

        public Startup(IConfiguration config)
        {
            _config = ConfigurationFactory.CreateAppConfigModel(config);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            LocalizationUtility.ConfigureTimeZone(_config.TimeZone);

            services.ConfigureServices(_config.SMSOptions);

            services.ConfigureDataAccess();

            services.ConfigureWeb(_config);

            services.ConfigureWorkers(_config.Workers);

            services.ConfigureEmailReporting(_config.SmtpSettings, _config.EmailSender, _config.Reports);

            services.AddSingleton(_config);

            if (_config.UseSwagger)
            {
                services.ConfigureSwagger();
            }

            if (_config.UseMiniProfiler)
            {
                services.ConfigureMiniProfiler();
            }
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (EnvironmentVariables.DOTNET_RUNNING_IN_CONTAINER)
            {
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost
                });
            }

            if (env.IsDevelopment())
            {
                app.UsePathBase("/dev");
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
                app.UsePathBase(_config.BasePath);
                app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api"), appBuilder =>
                {
                    appBuilder.UseStatusCodePagesWithReExecute($"/Bugs/Error", "?code={0}");
                    if (EnvironmentVariables.HARD_MODE)
                    {
                        appBuilder.UseMiddleware<SecurityHeadersMiddleware>();
                    }
                });
            }

            app.UseStaticFiles();

            if (_config.UseMiniProfiler)
            {
                app.UseMiniProfiler();
            }

            if (_config.UseSwagger)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"/swagger.json", $"BugzNet {AssemblyAttributes.GitTag} API");
                });
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api") && EnvironmentVariables.HARD_MODE, appBuilder =>
            {
                appBuilder.UseMiddleware<OtpMiddleware>();
                //appBuilder.UseMiddleware<MustChangePasswordMiddleware>();
            });
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", ctx =>
                {
                    ctx.Response.Redirect(ctx.Request.PathBase
                        + (ctx.User.Identity?.IsAuthenticated ?? false ? "/Bugs" : "/Identity/Login"));

                    return Task.CompletedTask;
                });

                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}