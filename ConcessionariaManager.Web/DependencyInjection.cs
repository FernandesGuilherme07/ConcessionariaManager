using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using ConcessionariaManager.Web.Data;
using ConcessionariaManager.Core.Interfaces.Services;
using ConcessionariaManager.Core.Services;
using StackExchange.Redis;
using ConcessionariaManager.Web.ExternalServices;
using ConcessionariaManager.Web.Data.Repositories;
using ConcessionariaManager.Core.Interfaces.Repositories;
using ConcessionariaManager.Web.Core.Interfaces.Services;
using ConcessionariaManager.Web.Data.Services;

namespace ConcessionariaManager.Web
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllersWithViews();
            services.AddEfCore(configuration);
            services.AddIdentity();
            services.AddLocalization();
            services.AddRazorPagesConfiguration();
            services.AddHttpContextAccessor();
            services.AddRepositories();
            services.AddRedis(configuration);
            services.AddClosedXML();
            services.AddServices();
            return services;
        }
        private static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedAccount = false;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_.@" + " ";
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddUserValidator<UserNameCustomValidation>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();
            
            return services;
        }

        private static IServiceCollection AddEfCore(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddScoped<ILogService, LogService>();
            services.AddScoped<LogSaveChangesInterceptor>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }
        private static IServiceCollection AddLocalization(this IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "pt-BR", "en-US" };
                options.DefaultRequestCulture = new RequestCulture("pt-BR");
                options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
                options.SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
            });

            return services;
        }
        private static IServiceCollection AddRazorPagesConfiguration(this IServiceCollection services)
        {
            services.AddRazorPages(options =>
            {
                options.Conventions.AllowAnonymousToPage("/Account/Login");
                options.Conventions.AllowAnonymousToPage("/Account/Register");
                options.Conventions.AuthorizeFolder("/Account");
                options.Conventions.AuthorizeFolder("/");
            });

            return services;
        }
        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped< IVendaService, VendaService>(); 

            return services;
        }
        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<IVendaRepository, VendaRepository>();
            services.AddScoped<IFabricanteRepository, FabricanteRepository>();
            return services;
        }
        private static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Redis")
                ?? throw new InvalidOperationException("Connection string 'Redis' not found.");

            services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(connectionString));

            services.AddScoped<ICacheService, RedisCacheService>();

            return services;
        }

        private static IServiceCollection AddClosedXML(this IServiceCollection services)
        {
            services.AddScoped<IExcelExportService, ClosedXMLExcelExportService>();

            return services;
        }
    }
}
