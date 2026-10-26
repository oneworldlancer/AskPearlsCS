using AskPearlsCS_PORTAL.Models;

using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

using AskPearlsCS_DOMAIN.app_Identity;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace AskPearlsCS_PORTAL.Controllers
{
    public class HomeController : Controller
    {


        private readonly UserManager<AppCore_IdentityUser> _userManager;
        private readonly SignInManager<AppCore_IdentityUser> _signInManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public HomeController(
            UserManager<AppCore_IdentityUser> userManager,
            SignInManager<AppCore_IdentityUser> signInManager,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }



        private static int _nextDesign = -1;
        private static readonly string[] DesignViews =
     {
            "Index_1",
            "Index_2",
            "Index_3",
            "Index_4",
            "Index_5",
            "Index_6",
            "Index_7",
            "Index_8"  ,
            "Index_9",
            "Index_10",
            "Index_11",
            "Index_12"//,
            //"Index_13",
            //"Index_14",
            //"Index_15"
        };

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {

            var index = Random.Shared.Next(DesignViews.Length);
          
          //   return View(DesignViews[index]);
           return View("Index_7");

            //var index = (int)((uint)Interlocked.Increment(ref _nextDesign) % (uint)DesignViews.Length);
            //return View(DesignViews[index]);
        }


        //public IActionResult Index()
        //{
        //    return View();
        //}





        [AllowAnonymous]
        [HttpGet]
        public IActionResult GoogleLogin(string? returnUrl = null)
        {
            var callbackUrl = Url.Action(
                nameof(GoogleCallback),
                "Home",
                new { returnUrl });

            var properties =
                _signInManager.ConfigureExternalAuthenticationProperties(
                    GoogleDefaults.AuthenticationScheme,
                    callbackUrl!);

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GoogleCallback(
            string? returnUrl = null,
            CancellationToken cancellationToken = default)
        {
            var externalInfo = await _signInManager.GetExternalLoginInfoAsync();

            if (externalInfo is null)
                return RedirectToAction(nameof(Index));

            var idToken =
                externalInfo.AuthenticationProperties?.GetTokenValue("id_token");

            if (string.IsNullOrWhiteSpace(idToken))
                return BadRequest("Google did not return an ID token.");

            var apiBaseUrl = _configuration["AskPearlsApi:BaseUrl"];

            if (string.IsNullOrWhiteSpace(apiBaseUrl))
                return Problem("AskPearls API URL is not configured.");

            var client = _httpClientFactory.CreateClient();

            using var apiResponse = await client.PostAsJsonAsync(
                $"{apiBaseUrl.TrimEnd('/')}/api/auth/google/validate",
                new { idToken },
                cancellationToken);

            if (!apiResponse.IsSuccessStatusCode)
                return Unauthorized("The API could not validate this Google account.");

            var validatedUser =
                await apiResponse.Content.ReadFromJsonAsync<GoogleAuthApiResult>(
                    cancellationToken);

            if (validatedUser is null)
                return Unauthorized("The API returned no user profile.");

            var identityUser = await _userManager.FindByLoginAsync(
                externalInfo.LoginProvider,
                externalInfo.ProviderKey);

            if (identityUser is null)
            {
                identityUser =
                    await _userManager.FindByEmailAsync(validatedUser.Email);

                if (identityUser is null)
                {
                    identityUser = new AppCore_IdentityUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = validatedUser.Email,
                        Email = validatedUser.Email,
                        EmailConfirmed = true
                    };

                    var createResult = await _userManager.CreateAsync(identityUser);

                    if (!createResult.Succeeded)
                        return Problem("Could not create the Portal Identity user.");
                }

                var addLoginResult = await _userManager.AddLoginAsync(
                    identityUser,
                    new UserLoginInfo(
                        externalInfo.LoginProvider,
                        externalInfo.ProviderKey,
                        externalInfo.ProviderDisplayName));

                if (!addLoginResult.Succeeded)
                    return Problem("Could not link the Google account to the Portal user.");
            }

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            await _signInManager.SignInWithClaimsAsync(
      identityUser,
      isPersistent: true,
      new[]
      {
        new Claim("AskPearlsUserId", validatedUser.UserId.ToString()),
        new Claim("AskPearlsName", validatedUser.Name ?? identityUser.UserName ?? "AskPearls user"),
        new Claim("AskPearlsEmail", validatedUser.Email)
      });


            if (!string.IsNullOrWhiteSpace(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return Redirect("/dashboard");
        }

        private sealed record GoogleAuthApiResult(
            long UserId,
            string Email,
            string? Name,
            bool IsNewUser);







        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index), "Home");
        }







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
