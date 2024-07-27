using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;
using ShieldNet.DependencyInjection.Cache;
using ShieldNet.DependencyInjection.Lazy;
using ShieldNet.Infras.Data.Contexts;
using ShieldNet.Infras.Services;
using ShieldNet.OAuth2.Endpoint;

namespace ShieldNet.WebHost.Extensions
{
    internal static class ConfigureServices
    {

        public static IServiceCollection RegisterAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add services to the container.
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddControllers().AddApplicationPart(typeof(AssemblyRef).Assembly);


            services.AddEntityFrameworkStores();
            services.AddOpenIddict();
            services.AddIdentity();

            {
                services.AddScoped<IEmailSender, EmailSender>();
                services.AddTransient<ILazyServiceProvider, LazyServiceProvider>();
                services.AddScoped<ICachedServiceProvider, CachedServiceProvider>();
                services.AddTransient<ICachedTransparentServiceProvider, CachedTransparentServiceProvider>();

                services.AddScoped<TestService>();
                services.AddTransient<WrapperService>();

            }

            services.AddHostedService<TestDataHostedService>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/account2/login";
                options.Cookie.Name = "__shieldnet.app.auth";
            });

            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = "__shieldnet.app.xsrf";
            });
            return services;
        }

        private static void AddIdentity(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                           .AddEntityFrameworkStores<AppDbContext>()
                           .AddDefaultTokenProviders();
        }

        private static void AddOpenIddict(this IServiceCollection services)
        {
            services.AddOpenIddict(oiddBuilder =>
            {
                oiddBuilder.AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                        .UseDbContext<AppDbContext>();
                });

                oiddBuilder.AddServer(options =>
                {
                    options
                        .AllowClientCredentialsFlow()
                        .AllowAuthorizationCodeFlow()
                        .AllowImplicitFlow()
                        .AllowPasswordFlow()
                        .AllowRefreshTokenFlow();

                    options
                        .SetTokenEndpointUris("/connect/token")
                        .SetAuthorizationEndpointUris("/connect/authorize");

                    options.RegisterScopes("api");

                    options.AddEphemeralEncryptionKey()
                            .AddEphemeralSigningKey()
                            .DisableAccessTokenEncryption()
                            ;

                    options.UseAspNetCore()
                        .EnableTokenEndpointPassthrough()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableLogoutEndpointPassthrough()
                        .EnableUserinfoEndpointPassthrough();
                });

                oiddBuilder.AddValidation(options =>
                {
                    options.UseLocalServer();
                    options.UseAspNetCore();

                });

            });
        }

        private static void AddEntityFrameworkStores(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(nameof(AppDbContext));

                options.UseOpenIddict();
            });
        }
    }
}
