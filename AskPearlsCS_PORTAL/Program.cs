
using AskPearlsCS_DOMAIN.app_Identity;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

using AskPearlsCS_PORTAL.app_Utility;
using AskPearlsCS_DOMAIN.app_Service;
using AskPearlsCS_BLL.app_Service;
using AskPearlsCS_DAL.app_Repository;
using AskPearlsCS_DAL.DBContext;

var builder = WebApplication.CreateBuilder(args);


// ============================================================
// 1. DATABASE CONTEXTS
// ============================================================

// Application database for DAL repositories
builder.Services.AddDbContext<DBContextModel>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ASKPEARLSDB")
    )
);

// Identity database
builder.Services.AddDbContext<AppCore_DBContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ASKPEARLSDB")
    )
);


// ============================================================
// 2. ASP.NET CORE IDENTITY
// ============================================================

builder.Services
    .AddIdentity<AppCore_IdentityUser, AppCore_IdentityRole>(
        options =>
        {
            options.User.RequireUniqueEmail = true;
        })
    .AddEntityFrameworkStores<AppCore_DBContext>()
    .AddDefaultTokenProviders();


// ============================================================
// 3. AUTHENTICATION COOKIE
// ============================================================

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".AskPearls.Portal.Auth";

    // Authentication ticket lifetime: 10 hours
    options.ExpireTimeSpan = TimeSpan.FromHours(10);

    // Renew the authentication ticket during eligible requests
    options.SlidingExpiration = true;

    // Authentication routes
    options.LoginPath = "/Home/Index";

    // Secure cookie configuration
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});


// ============================================================
// 4. GOOGLE OAUTH AUTHENTICATION
// ============================================================

builder.Services
    .AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId =
            builder.Configuration["GoogleOAuth:ClientId"]!;

        options.ClientSecret =
            builder.Configuration["GoogleOAuth:ClientSecret"]!;

        options.SignInScheme = IdentityConstants.ExternalScheme;

        options.SaveTokens = true;

        options.Events.OnCreatingTicket = context =>
        {
            var response = context.TokenResponse.Response;

            if (response is not null &&
                response.RootElement.TryGetProperty(
                    "id_token",
                    out var tokenElement))
            {
                var idToken = tokenElement.GetString();

                if (!string.IsNullOrWhiteSpace(idToken))
                {
                    var tokens = context.Properties
                        .GetTokens()
                        .ToList();

                    tokens.RemoveAll(
                        token => token.Name == "id_token"
                    );

                    tokens.Add(new AuthenticationToken
                    {
                        Name = "id_token",
                        Value = idToken
                    });

                    context.Properties.StoreTokens(tokens);
                }
            }

            return Task.CompletedTask;
        };
    });


// ============================================================
// 5. USER CONTEXT AND APPLICATION SERVICES
// ============================================================

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<
    ICurrentUserContext_Service,
    CurrentUserContext_Service>();

builder.Services.AddScoped<
    ICurrentUser_Service,
    CurrentUser_Service>();

builder.Services.AddScoped<
    ICurrentUser_Repository,
    CurrentUser_Repository>();

builder.Services.AddScoped<
    IApiJwtToken_Service,
    ApiJwtToken_Service>();

builder.Services.AddScoped<ApiAccessTokenService>();


// ============================================================
// 6. ANTIFORGERY
// ============================================================

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});


// ============================================================
// 7. HTTP CLIENT FOR ASKPEARLS API
// ============================================================

builder.Services
    .AddHttpClient("AskPearlsWidgetApi", client =>
    {
        client.Timeout = TimeSpan.FromSeconds(60);
    })
    .ConfigurePrimaryHttpMessageHandler(() =>
        new HttpClientHandler
        {
            AllowAutoRedirect = false
        });


// ============================================================
// 8. MVC
// ============================================================

builder.Services.AddControllersWithViews();


// ============================================================
// 9. SESSION SUPPORT FOR iOwnerModel
// ============================================================

// In-memory session storage
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    // Session idle timeout: 10 hours
    options.IdleTimeout = TimeSpan.FromHours(10);

    options.Cookie.Name = ".AskPearls.Portal.Session";

    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});


// ============================================================
// 10. BUILD APPLICATION
// ============================================================

var app = builder.Build();


// ============================================================
// 11. ERROR HANDLING AND HTTPS
// ============================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();


// ============================================================
// 12. REQUEST PIPELINE
// ============================================================

app.UseRouting();

// Load ASP.NET Core Session
app.UseSession();

// Authenticate the logged-in user
app.UseAuthentication();

// Apply authorization
app.UseAuthorization();


// ============================================================
// 13. STATIC FILES
// ============================================================

app.MapStaticAssets();


// ============================================================
// 14. MVC ROUTES
// ============================================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


// ============================================================
// 15. START APPLICATION
// ============================================================

app.Run();
