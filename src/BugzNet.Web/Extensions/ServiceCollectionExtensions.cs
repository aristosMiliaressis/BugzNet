using AutoMapper;
using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using BugzNet.Core.ValueObjects;
using BugzNet.Infrastructure.DataJson;
using BugzNet.Infrastructure.DataEF;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using BugzNet.Infrastructure.Configuration;
using BugzNet.Web.Authorization;
using BugzNet.Web.InputFormatters;
using Microsoft.FeatureManagement;
using BugzNet.Infrastructure.Configuration.Models;
using Microsoft.Extensions.Hosting;
using BugzNet.Infrastructure.DataJson.Interfaces;
using BugzNet.Infrastructure.DataEF.Repositories.Interfaces;
using BugzNet.Infrastructure.Validators;
using BugzNet.Infrastructure.MediatR.PipelineBehaviors;
using BugzNet.Infrastructure.Email;
using Microsoft.Extensions.Logging;
using BugzNet.Core.Utilities;
using BugzNet.Infrastructure.Extensions;
using StackExchange.Profiling.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FluentValidation.AspNetCore;
using BugzNet.Core.Entities.Identity;
using BugzNet.Infrastructure.SMS;

namespace BugzNet.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private readonly static ILogger _logger = LoggingUtility.LoggerFactory.CreateLogger(nameof(ServiceCollectionExtensions));

        public static void ConfigureServices(this IServiceCollection services, SMSOptions smsOptions)
        {
            services.AddAutoMapper(AssemblyAttributes.CustomAssemblies);

            #region MediatR
            services.AddMediatR(AssemblyAttributes.CustomAssemblies);

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ModelValidationBehaviour<,>));
            #endregion

            services.AddTransient<ISmsSender, SMSSender>();
            services.AddSingleton<SMSOptions>(smsOptions);

            services.AddFeatureManagement();
        }

        public static void ConfigureWorkers(this IServiceCollection services, List<WorkerConfig> workerConfig)
        {
            try
            {
                foreach (var worker in workerConfig)
                {
                    var workerType = AssemblyAttributes.CustomAssemblies.SelectMany(a => a.GetTypes()).Where(t => t.GetInterface(nameof(IHostedService)) != null && t.Name == worker.Name).First();
                    var addHostedServiceMethod = typeof(ServiceCollectionHostedServiceExtensions).GetMethod(nameof(ServiceCollectionHostedServiceExtensions.AddHostedService),
                                                                                    new Type[] { typeof(IServiceCollection) });
                    addHostedServiceMethod = addHostedServiceMethod.MakeGenericMethod(workerType);
                    addHostedServiceMethod.Invoke(null, new object[] { services });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error while configuring hosted services");
                _logger.LogException(ex);
            }            
        }

        public static void ConfigureEmailReporting(this IServiceCollection services, SmtpSettings smtpSettings, EmailSender emailSender, List<ReportSettings> reportSettings)
        {
            // SMTP sender config
            services.AddSingleton<IReportSender, SmtpReportSender>(sp => (SmtpReportSender)Activator.CreateInstance(typeof(SmtpReportSender), new object[] { smtpSettings, emailSender }));

            // Microsoft Graph sender config
            //services.AddSingleton<IReportSender, MicrosoftGraphReportSender>(sp => (MicrosoftGraphReportSender)Activator.CreateInstance(typeof(MicrosoftGraphReportSender), new object[] { appConfig.SmtpSettings, appConfig.EmailSender }));

            services.AddSingleton(typeof(List<ReportSettings>), sp => reportSettings);
        }

        public static void ConfigureMiniProfiler(this IServiceCollection services)
        {
            services.AddMiniProfiler(options =>
            {
                // All of this is optional. You can simply call .AddMiniProfiler() for all defaults

                // (Optional) Path to use for profiler URLs, default is /mini-profiler-resources
                options.RouteBasePath = "/profiler";

                // (Optional) Control storage
                // (default is 30 minutes in MemoryCacheStorage)
                // Note: MiniProfiler will not work if a SizeLimit is set on MemoryCache!
                //   See: https://github.com/MiniProfiler/dotnet/issues/501 for details
                (options.Storage as MemoryCacheStorage).CacheDuration = TimeSpan.FromMinutes(60);

                // (Optional) Control which SQL formatter to use, InlineFormatter is the default
                options.SqlFormatter = new StackExchange.Profiling.SqlFormatters.InlineFormatter();

                // (Optional) To control authorization, you can use the Func<HttpRequest, bool> options:
                // (default is everyone can access profilers)
                //options.ResultsAuthorize = request => MyGetUserFunction(request).CanSeeMiniProfiler;
                //options.ResultsListAuthorize = request => MyGetUserFunction(request).CanSeeMiniProfiler;
                // Or, there are async versions available:
                //options.ResultsAuthorizeAsync = async request => (await MyGetUserFunctionAsync(request)).CanSeeMiniProfiler;
                //options.ResultsAuthorizeListAsync = async request => (await MyGetUserFunctionAsync(request)).CanSeeMiniProfilerLists;

                // (Optional)  To control which requests are profiled, use the Func<HttpRequest, bool> option:
                // (default is everything should be profiled)
                //options.ShouldProfile = request => MyShouldThisBeProfiledFunction(request);

                // (Optional) Profiles are stored under a user ID, function to get it:
                // (default is null, since above methods don't use it by default)
                //options.UserIdProvider = request => MyGetUserIdFunction(request);

                // (Optional) Swap out the entire profiler provider, if you want
                // (default handles async and works fine for almost all applications)
                //options.ProfilerProvider = new MyProfilerProvider();

                // (Optional) You can disable "Connection Open()", "Connection Close()" (and async variant) tracking.
                // (defaults to true, and connection opening/closing is tracked)
                options.TrackConnectionOpenClose = true;

                // (Optional) Use something other than the "light" color scheme.
                // (defaults to "light")
                options.ColorScheme = StackExchange.Profiling.ColorScheme.Auto;

                // Optionally change the number of decimal places shown for millisecond timings.
                // (defaults to 2)
                //options.PopupDecimalPlaces = 1;

                // The below are newer options, available in .NET Core 3.0 and above:

                // (Optional) You can disable MVC filter profiling
                // (defaults to true, and filters are profiled)
                options.EnableMvcFilterProfiling = true;
                // ...or only save filters that take over a certain millisecond duration (including their children)
                // (defaults to null, and all filters are profiled)
                // options.MvcFilterMinimumSaveMs = 1.0m;

                // (Optional) You can disable MVC view profiling
                // (defaults to true, and views are profiled)
                options.EnableMvcViewProfiling = true;
                // ...or only save views that take over a certain millisecond duration (including their children)
                // (defaults to null, and all views are profiled)
                // options.MvcViewMinimumSaveMs = 1.0m;

                // (Optional) listen to any errors that occur within MiniProfiler itself
                // options.OnInternalError = e => MyExceptionLogger(e);

                // (Optional - not recommended) You can enable a heavy debug mode with stacks and tooltips when using memory storage
                // It has a lot of overhead vs. normal profiling and should only be used with that in mind
                // (defaults to false, debug/heavy mode is off)
                //options.EnableDebugMode = true;
            }).AddEntityFramework();
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                //c.OperationFilter<AuthorizationOperationFilter>();

                c.SwaggerDoc("main", new OpenApiInfo { Title = "BugzNet+ API", Version = AssemblyAttributes.GitTag });

                // Add JWT Authentication
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter JWT Bearer token **_only_**",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = "JWT",
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                // Add ApiKey Authentication
                var apiKeySecurityScheme = new OpenApiSecurityScheme
                {
                    Name = "ApiKey Authentication",
                    Description = "Enter ApiKey Bearer token **_only_**",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "ApiKey",
                    Reference = new OpenApiReference
                    {
                        Id = "ApiKey",
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(apiKeySecurityScheme.Reference.Id, apiKeySecurityScheme);

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        public static void ConfigureDataAccess(this IServiceCollection services)
        {
            services.AddDbContextPool<BugzNetDataContext>(o => BugzNetDataContext.GetOptionsBuilder(o));

            services.AddTransient<DatabaseInitializer, DatabaseInitializer>();
            services.AddScoped<IDbExistanceValidator, DbExistanceValidator>();

            if (!EnvironmentVariables.DOTNET_RUNNING_IN_CONTAINER)
            {
                services.AddDataProtection();
            }

            #region Identity
            services.AddIdentity<BugzUser, BugzRole>()
                .AddEntityFrameworkStores<BugzNetDataContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            });

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromSeconds(10);
            });

            services.Configure<PasswordHasherOptions>(option =>
            {
                option.IterationCount = 12000;
            });
            #endregion

            #region Repositories
            services.AddScoped<IValueObjectRepository<Currency>, CurrencyRepository>();

            services.AddScoped<IValueObjectRepository<Country>, CountryRepository>();
            services.Decorate<IValueObjectRepository<Country>, CachedCountryRepository>();

            services.AddScoped<IValueObjectRepository<Bug>, BugRepository>();

            services.AddClassesAsImplementedInterface(typeof(BugzNetDataContext).Assembly, typeof(IEntityRepository<>));
            #endregion
        }

        public static void ConfigureWeb(this IServiceCollection services, AppConfig appConfig)
        {
            services.AddMvcCore().AddApiExplorer();

            var mvcBuilder = services.AddControllersWithViews(o =>
            {
                o.RespectBrowserAcceptHeader = true;
                o.InputFormatters.Insert(o.InputFormatters.Count, new TextPlainInputFormatter());
            }).AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblies(AssemblyAttributes.CustomAssemblies));

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
            });

            (string prefix, CookieSecurePolicy securePolicy) = EnvironmentVariables.ASPNETCORE_URLS != null 
                                                            && !EnvironmentVariables.ASPNETCORE_URLS.Contains("https://")
                ? ("", CookieSecurePolicy.SameAsRequest)
                : ("__Secure-", CookieSecurePolicy.Always);

            services.AddAntiforgery(o =>
            {
                o.FormFieldName = "xsrf_token";
                o.Cookie.Name = prefix + "Xsrf-Token";
                o.Cookie.SecurePolicy = securePolicy;
                o.Cookie.HttpOnly = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Bugs/Error?code=403";
                options.Cookie.Name = prefix + "BugzNet-Session";
                options.Cookie.SecurePolicy = securePolicy;
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(appConfig?.AuthTimeOutMin ?? 20);
                options.LoginPath = "/Identity/Login";
                options.LogoutPath = "/Identity/Login?handler=Logout";
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(o => { o.LoginPath = "/Identity/Login"; o.Cookie.Name = prefix + "BugzNet-Session";})
                    .AddJwtBearer(jwt =>
                    {
                        jwt.TokenValidationParameters = new TokenValidationParameters
                        {
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appConfig.TokenSigningKey)),
                            ValidAudience = "BugzAudience",
                            ValidIssuer = "BugzIssuer",
                            ValidateAudience = true,
                            ValidateIssuer = true,
                            ValidateIssuerSigningKey = true,
                            // When receiving a token, check that it is still valid.
                            ValidateLifetime = true,
                            // This defines the maximum allowable clock skew - i.e. provides a tolerance on the token expiry time
                            // when validating the lifetime. As we're creating the tokens locally and validating them on the same
                            // machines which should have synchronised time, this can be set to zero. Where external tokens are
                            // used, some leeway here could be useful.
                            ClockSkew = TimeSpan.FromMinutes(0),
                            RequireExpirationTime = true
                        };
                    });

            // TODO: readonly user rbac
            // TODO: make AccessDenied to 404
            services.AddAuthorization(options =>
            {
                options.AddPolicy(nameof(AuthorizationPolicies.SuperUserPolicy), AuthorizationPolicies.SuperUserPolicy);
                options.AddPolicy(nameof(AuthorizationPolicies.UIUserPolicy), AuthorizationPolicies.UIUserPolicy);
                options.AddPolicy(nameof(AuthorizationPolicies.JwtApiPolicy), AuthorizationPolicies.JwtApiPolicy);
                options.AddPolicy(nameof(AuthorizationPolicies.ApiKeyPolicy), AuthorizationPolicies.ApiKeyPolicy(appConfig.WebRequestStaticToken));

                options.DefaultPolicy = AuthorizationPolicies.UIUserPolicy;
                options.FallbackPolicy = AuthorizationPolicies.UIUserPolicy;
            });

            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder("/");
                options.Conventions.AuthorizeFolder("/Admin", nameof(AuthorizationPolicies.SuperUserPolicy));
                options.Conventions.AllowAnonymousToFolder("/Identity");
            });
            
#if DEBUG
            mvcBuilder.AddRazorRuntimeCompilation();
#endif
        }

        private static List<TypeInfo> GetTypesAssignableTo(this Assembly assembly, Type compareType)
        {
            var typeInfoList = assembly.DefinedTypes.Where(x => x.IsClass
                                && !x.IsAbstract
                                && x != compareType
                                && x.GetInterfaces()
                                    .Any(i => i.IsGenericType
                                           && i.GetGenericTypeDefinition() == compareType))?.ToList();

            return typeInfoList;
        }

        private static void AddClassesAsImplementedInterface(this IServiceCollection services, Assembly assembly, Type compareType, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            assembly.GetTypesAssignableTo(compareType).ForEach((type) =>
            {
                foreach (var implementedInterface in type.ImplementedInterfaces)
                {
                    switch (lifetime)
                    {
                        case ServiceLifetime.Scoped:
                            services.AddScoped(implementedInterface, type);
                            break;
                        case ServiceLifetime.Singleton:
                            services.AddSingleton(implementedInterface, type);
                            break;
                        case ServiceLifetime.Transient:
                            services.AddTransient(implementedInterface, type);
                            break;
                    }
                }
            });
        }
    }
}