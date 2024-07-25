using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using ShieldNet.DependencyInjection.Lazy;
using ShieldNet.Domain.User;
namespace ShieldNet.OAuth2.Endpoint.Controllers;

public abstract class OAuth2ControllerBase
    (ILazyServiceProvider lazyServiceProvider) : Controller
{
    public ILazyServiceProvider LazyServiceProvider { get; } = lazyServiceProvider;

    protected IOpenIddictApplicationManager ApplicationManager => LazyServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
    protected IOpenIddictScopeManager ScopeManager => LazyServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
    protected IOpenIddictTokenManager TokenManager => LazyServiceProvider.GetRequiredService<IOpenIddictTokenManager>();
    protected IOpenIddictAuthorizationManager AuthorizationManager => LazyServiceProvider.GetRequiredService<IOpenIddictAuthorizationManager>();

    protected UserManager<ApplicationUser> UserManager => LazyServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    protected SignInManager<ApplicationUser> SignInManager => LazyServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
}

