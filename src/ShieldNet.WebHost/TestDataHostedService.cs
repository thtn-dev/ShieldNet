using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using ShieldNet.Infras.Data.Contexts;

namespace ShieldNet.WebHost
{
    public class TestDataHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        public TestDataHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await SeedClientsAsync(cancellationToken);
            await SeedUsersAsync(cancellationToken);
        }

        public async Task SeedUsersAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<TestDataHostedService>>();

            var user = await userManager.FindByNameAsync("admin");
            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "thtntrungnam@gmail.com",
                    EmailConfirmed = true,
                    Id = 9999999,
                    LockoutEnabled = false,

                };
                var result = await userManager.CreateAsync(user, "Admin#123");
                if (result.Succeeded)
                {
                    logger.LogInformation("Admin user created");
                }
            }
        }

        public async Task SeedClientsAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<TestDataHostedService>>();

            if (await manager.FindByClientIdAsync("postman", cancellationToken) is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "postman",
                    ClientSecret = "postman-secret",
                    DisplayName = "Postman",
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                         OpenIddictConstants.Permissions.GrantTypes.Password,

                        OpenIddictConstants.Permissions.Prefixes.Scope + "api"
                    }
                }, cancellationToken);
                logger.LogInformation("Postman application created");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
