using Microsoft.AspNetCore.Mvc;
using ShieldNet.DependencyInjection.Lazy;

namespace ShieldNet.OAuth2.Endpoint.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("connect/token")]
[IgnoreAntiforgeryToken]
public partial class TokenController : OAuth2ControllerBase
{
    public TokenController(ILazyServiceProvider lazyServiceProvider) : base(lazyServiceProvider)
    {
    }
}