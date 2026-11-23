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
using AskPearlsCS_DOMAIN.app_Service;
using AskPearlsCS_BLL.app_Service;

namespace AskPearlsCS_PORTAL.Controllers
{
    public class HomeController : Controller
    {

        private readonly ICurrentUser_Service _currentUser;

        private readonly UserManager<AppCore_IdentityUser> _userManager;
        private readonly SignInManager<AppCore_IdentityUser> _signInManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public HomeController(
            UserManager<AppCore_IdentityUser> userManager,
            SignInManager<AppCore_IdentityUser> signInManager,
            ICurrentUser_Service currentUser,
         IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _currentUser = currentUser;
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
            "Index_12",
            "Index_13",
            "Index_14",
            "Index_15"
        };



        private static readonly string[] HeaderSlogans =
        {
    "Capture. Engage. Convert.",
    "Connect. Discover. Grow.",
    "Imagine. Build. Succeed.",
    "Explore. Decide. Achieve.",
    "Discover. Connect. Thrive.",
    "Create. Collaborate. Innovate.",
    "Think. Plan. Execute.",
    "Inspire. Empower. Transform.",
    "Capture. Understand. Deliver.",
    "Simplify. Connect. Succeed.",
    "Explore. Learn. Grow.",
    "Connect. Analyze. Decide.",
    "Engage. Inspire. Convert.",
    "Discover. Decide. Deliver.",
    "Listen. Understand. Act.",
    "Collect. Analyze. Improve.",
    "Imagine. Connect. Create.",
    "Understand. Decide. Grow.",
    "Capture. Connect. Succeed.",
    "Explore. Innovate. Transform.",
    "Connect. Empower. Achieve.",
    "Discover. Engage. Thrive.",
    "Think. Create. Grow.",
    "Simplify. Empower. Deliver.",
    "Learn. Adapt. Succeed.",
    "Create. Connect. Convert.",
    "Discover. Design. Deliver.",
    "Imagine. Explore. Achieve.",
    "Analyze. Optimize. Grow.",
    "Connect. Collaborate. Succeed."
};

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Index()
        {
            // Random homepage design (1–15)
            int designIndex = Random.Shared.Next(DesignViews.Length);

            // Random three-word slogan
            int sloganIndex = Random.Shared.Next(HeaderSlogans.Length);

            // Available to the shared header partial
            ViewData["HeaderSlogan"] = HeaderSlogans[sloganIndex];

            // Render the selected homepage
         
         //     return View(DesignViews[designIndex]);
 return View("Index_13");

        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Index()
        //{

        //    var index = Random.Shared.Next(DesignViews.Length);

        //  return View(DesignViews[index]);
        //   //   return View("Index_15");

        //    //var index = (int)((uint)Interlocked.Increment(ref _nextDesign) % (uint)DesignViews.Length);
        //    //return View(DesignViews[index]);
        //}


        //public IActionResult Index()
        //{
        //    return View();
        //}


        private async Task<IdentityResult> SyncAskPearlsClaimsAsync(
            AppCore_IdentityUser identityUser,
            string ownerUserTokenId,
            string? firstName,
            string? lastName,
            string email)
        {
            if (string.IsNullOrWhiteSpace(ownerUserTokenId))
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "MissingAskPearlsUserId",
                        Description = "AskPearls user ID is missing."
                    });
            }

            var fullName = string.Join(" ",
                new[] { firstName, lastName }
                    .Where(x => !string.IsNullOrWhiteSpace(x)));

            var desiredClaims = new Dictionary<string, string>
            {
                [AskPearlsClaimTypes.ASKPEARLS_USER_ID] =
                    ownerUserTokenId,

                [AskPearlsClaimTypes.ASKPEARLS_USER_FIRST_NAME] =
                    firstName ?? string.Empty,

                [AskPearlsClaimTypes.ASKPEARLS_USER_LAST_NAME] =
                    lastName ?? string.Empty,

                [AskPearlsClaimTypes.ASKPEARLS_USER_FULL_NAME] =
                    fullName,

                [AskPearlsClaimTypes.ASKPEARLS_USER_EMAIL] =
                    email
            };

            var existingClaims =
                await _userManager.GetClaimsAsync(identityUser);

            foreach (var item in desiredClaims)
            {
                var matches = existingClaims
                    .Where(c => c.Type == item.Key)
                    .ToList();

                if (matches.Count == 1 &&
                    matches[0].Value == item.Value)
                {
                    continue;
                }

                if (matches.Count > 0)
                {
                    var removeResult =
                        await _userManager.RemoveClaimsAsync(
                            identityUser,
                            matches);

                    if (!removeResult.Succeeded)
                        return removeResult;
                }

                var addResult =
                    await _userManager.AddClaimAsync(
                        identityUser,
                        new Claim(item.Key, item.Value));

                if (!addResult.Succeeded)
                    return addResult;
            }

            return IdentityResult.Success;
        }




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


            await HttpContext.SignOutAsync(
                IdentityConstants.ExternalScheme);

            // Load the actual AskPearls application user.
            var userModel =
                await _currentUser.Get_UserByEmailAddressAsync(
                    validatedUser.Email,
                    cancellationToken);

            if (userModel is null ||
                string.IsNullOrWhiteSpace(userModel.OwnerUserTokenID))
            {
                return Problem(
                    "The authenticated Google account does not have " +
                    "a valid AskPearls user record.");
            }

            // Persist the claims in ASP.NET Core Identity.
            // They will be available when Identity regenerates
            // the authentication principal.
            var claimsResult = await SyncAskPearlsClaimsAsync(
                identityUser,
                userModel.OwnerUserTokenID,
                userModel.FirstName,
                userModel.LastName,
                validatedUser.Email);

            if (!claimsResult.Succeeded)
            {
                return Problem(
                    "Could not synchronize the AskPearls user claims.");
            }

            // Sign in using the normal Identity principal.
            // The persisted claims are included automatically
            // by the standard Identity claims-principal factory.
            await _signInManager.SignInAsync(
                identityUser,
                isPersistent: true);


            //////////      await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ////////// var userModel = await _currentUser.Get_UserByEmailAddressAsync(validatedUser.Email, CancellationToken.None);

            //////////      await _signInManager.SignInWithClaimsAsync(
            //////////identityUser,
            //////////isPersistent: true,
            //////////new[]
            //////////{
            //////////  new Claim(AskPearlsClaimTypes.ASKPEARLS_USER_ID, userModel.OwnerUserTokenID),
            //////////  new Claim(AskPearlsClaimTypes.ASKPEARLS_USER_FIRST_NAME, userModel.FirstName),
            //////////  new Claim(AskPearlsClaimTypes.ASKPEARLS_USER_LAST_NAME, userModel.LastName),
            //////////  new Claim(AskPearlsClaimTypes.ASKPEARLS_USER_FULL_NAME, userModel.FirstName + " " + userModel.LastName),
            //////////  new Claim(AskPearlsClaimTypes.ASKPEARLS_USER_EMAIL, validatedUser.Email)
            //////////});


            //      await _signInManager.SignInWithClaimsAsync(
            //identityUser,
            //isPersistent: true,
            //new[]
            //{
            //  new Claim(AskPearlsClaimTypes.ASKPEARLS_USER_ID, validatedUser.UserId.ToString()),
            //  new Claim(AskPearlsClaimTypes.ASKPEARLS_USER_NAME, validatedUser.Name ?? identityUser.UserName ?? "AskPearls user"),
            //  new Claim(AskPearlsClaimTypes.ASKPEARLS_USER_EMAIL, validatedUser.Email)
            //});


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
