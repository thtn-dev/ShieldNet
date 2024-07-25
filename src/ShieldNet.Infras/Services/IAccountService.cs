using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShieldNet.Application.Services;
using ShieldNet.DependencyInjection.Lazy;
using ShieldNet.Domain.User;


namespace ShieldNet.Infras.Services;
 public partial class AccountService : IAccountService
{
    public ILazyServiceProvider LazyServiceProvider { get; }
    public AccountService(ILazyServiceProvider lazyServiceProvider)
    {
        LazyServiceProvider = lazyServiceProvider;
    }
    private SignInManager<ApplicationUser> signInManager => LazyServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
    private UserManager<ApplicationUser> userManager => LazyServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    private IUserStore<ApplicationUser> userStore => LazyServiceProvider.GetRequiredService<IUserStore<ApplicationUser>>();

    private IUserEmailStore<ApplicationUser> emailStore => GetEmailStore();

    private IEmailSender emailSender => LazyServiceProvider.GetRequiredService<IEmailSender>();

    private  ILogger<AccountService> logger => LazyServiceProvider.GetRequiredService<ILogger<AccountService>>();

    private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<ApplicationUser>)userStore;
    }
}

public partial class AccountService
{
}