using Microsoft.AspNetCore.Mvc;
using ShieldNet.DependencyInjection.Lazy;

namespace ShieldNet.OAuth2.Endpoint.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("connect/authorize")]
public class AuthorizeController : OAuth2ControllerBase
{
    public AuthorizeController(ILazyServiceProvider lazyServiceProvider) : base(lazyServiceProvider)
    {
    }
}