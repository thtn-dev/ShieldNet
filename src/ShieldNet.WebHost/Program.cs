using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;
using ShieldNet.Domain.User;
using ShieldNet.Infras.Data.Contexts;
using Vite.AspNetCore;

namespace ShieldNet.WebHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            builder.Services.AddViteServices(options =>
            {
                options.Server.AutoRun = true;
                options.Server.Https = true;
            });


            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(nameof(AppDbContext));

                options.UseOpenIddict();
            });

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();


            var oiddBuilder = builder.Services.AddOpenIddict();
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

            builder.Services.AddHostedService<TestDataHostedService>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapRazorPages();
                    endpoints.MapControllers();
                    endpoints.MapControllerRoute(
                                           name: "default",
                                           pattern: "{controller=Home}/{action=Index}/{id?}");
                });
            if (app.Environment.IsDevelopment())
            {
                app.UseWebSockets();
                // Use Vite Dev Server as middleware.
                app.UseViteDevelopmentServer(true);
            }
            app.Run();
        }
    }
}
