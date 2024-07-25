using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using ShieldNet.DependencyInjection.Lazy;
using ShieldNet.OAuth2.Endpoint.Controllers;
using ShieldNet.WebHost.Controllers;
using ShieldNet.WebHost.Models;
using System.Diagnostics;

namespace ShieldNet.WebHost.Controllers
{
    public class HomeController : Controller
    {
        private ILogger<HomeController> _logger => LazyServiceProvider.GetRequiredService<ILogger<HomeController>>();
        private TestService _testService => LazyServiceProvider.GetRequiredService<TestService>();

        private WrapperService _wrapperService;

        public ILazyServiceProvider LazyServiceProvider { get; }

        public HomeController(ILazyServiceProvider lazyServiceProvider, WrapperService wrapperService)
        {
            LazyServiceProvider = lazyServiceProvider;
            _wrapperService = wrapperService;
        }

        public IActionResult Index()
        {
            _testService.Test("Index");
            _wrapperService.testService.Test("Index2");
            return View();
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

public class WrapperService
{
    public readonly TestService testService;
    public WrapperService(TestService testService)
    {
        this.testService = testService;
    }
}

public class TestService
{
    private ILogger<HomeController> _logger => LazyServiceProvider.GetRequiredService<ILogger<HomeController>>();

    public ILazyServiceProvider LazyServiceProvider { get; }

    public TestService(ILazyServiceProvider lazyServiceProvider)
    {
        LazyServiceProvider = lazyServiceProvider;
    }

    public void Test(string call)
    {
        _logger.LogInformation(call);
        _logger.LogInformation(this.GetHashCode()+"");
    }
}