using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShieldNet.DependencyInjection.Lazy;
using ShieldNet.WebHost.Areas.Identity.ViewModels;

namespace ShieldNet.WebHost.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("[controller]/{action=Index}")]
    public class Account2Controller : Controller
    {
        public ILazyServiceProvider LazyServiceProvider { get; }

        private SignInManager<ApplicationUser> SignInManager => LazyServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
        public Account2Controller(ILazyServiceProvider lazyServiceProvider)
        {
            LazyServiceProvider = lazyServiceProvider;
        }

        [TempData]
        public string? ErrorMessage { get; set; }

        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Login([FromQuery] string returnUrl)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            returnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            var model = await BuildLoginViewModel(returnUrl);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginInputModel input, [FromQuery] string? returnUrl)
        {
            returnUrl ??= Url.Content("~/");
            var model = await BuildLoginViewModel(returnUrl);
            if (ModelState.IsValid)
            {
                var result = await SignInManager.PasswordSignInAsync(input.Email, input.Password, input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }
            return View(model);
        }

        private async Task<LoginViewModel> BuildLoginViewModel(string returnUrl)
        {
            return new LoginViewModel()
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await SignInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
            };
        }
    }
}
