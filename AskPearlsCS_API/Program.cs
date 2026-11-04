using System.Security.Claims;

using AskPearlsCS_BLL.DecisionOSManager;
using AskPearlsCS_BLL.WidgetDecision;
using AskPearlsCS_BLL.app_Service;

using AskPearlsCS_DAL.app_Repository;
using AskPearlsCS_DAL.DBContext;
using AskPearlsCS_DAL.DBModels;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// DecisionOS services and repositories
builder.Services.AddScoped<IDecisionEngine, DecisionEngine>();

builder.Services.AddScoped<IWidgetDecisionService, WidgetDecisionService>();
builder.Services.AddScoped<IWidget_Repository, Widget_Repository>();
builder.Services.AddScoped<IWidget_Service, Widget_Service>();

builder.Services.AddScoped<IAnswer_Repository, Answer_Repository>();
builder.Services.AddScoped<IAnswer_Service, Answer_Service>();

builder.Services.AddScoped<IWidgetVersion_Repository, WidgetVersion_Repository>();
builder.Services.AddScoped<IWidgetVersion_Service, WidgetVersion_Service>();

builder.Services.AddScoped<IQuestion_Repository, Question_Repository>();
builder.Services.AddScoped<IQuestion_Service, Question_Service>();

builder.Services.AddScoped<IDecisionRule_Repository, DecisionRule_Repository>();
builder.Services.AddScoped<IDecisionRule_Service, DecisionRule_Service>();

builder.Services.AddScoped<IRecommendationItem_Repository, RecommendationItem_Repository>();
builder.Services.AddScoped<IRecommendationItem_Service, RecommendationItem_Service>();

builder.Services.AddScoped<ICurrentUser_Repository, CurrentUser_Repository>();
builder.Services.AddScoped<ICurrentUser_Service, CurrentUser_Service>();

builder.Services.AddScoped<IGoogleAuth_Repository, GoogleAuth_Repository>();
builder.Services.AddScoped<IGoogleAuth_Service, GoogleAuth_Service>();

// Application database
builder.Services.AddDbContext<DBContextModel>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ASKPEARLSDB"));
});

// Google authentication uses the API's cookie flow.
// ApiJwt remains a named scheme for Portal-to-API requests.
var jwt = builder.Configuration.GetSection("ApiJwt");

var jwtIssuer = jwt["Issuer"]
    ?? throw new InvalidOperationException("ApiJwt:Issuer is missing.");

var jwtAudience = jwt["Audience"]
    ?? throw new InvalidOperationException("ApiJwt:Audience is missing.");

var SigningKey = jwt["SigningKey"]
    ?? throw new InvalidOperationException("ApiJwt:SigningKey is missing.");

byte[] signingKeyBytes;

try
{
    signingKeyBytes = Convert.FromBase64String(SigningKey);
}
catch (FormatException exception)
{
    throw new InvalidOperationException(
        "ApiJwt:SigningKey must contain a valid Base64 key.",
        exception);
}

if (signingKeyBytes.Length < 32)
{
    throw new InvalidOperationException(
        "ApiJwt:SigningKey must decode to at least 32 bytes.");
}

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["GoogleOAuth:ClientId"]
            ?? throw new InvalidOperationException("GoogleOAuth:ClientId is missing.");

        options.ClientSecret = builder.Configuration["GoogleOAuth:ClientSecret"]
            ?? throw new InvalidOperationException("GoogleOAuth:ClientSecret is missing.");

        options.CallbackPath = "/signin-google";
    })
    .AddJwtBearer("ApiJwt", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),

            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Widget", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("Widget");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/api/auth/google/start", () =>
{
    var properties = new AuthenticationProperties
    {
        RedirectUri = "/api/auth/google/complete"
    };

    return Results.Challenge(
        properties,
        new[] { GoogleDefaults.AuthenticationScheme });
});

app.MapGet("/api/auth/google/complete", async (
    HttpContext context,
    DBContextModel db) =>
{
    if (context.User.Identity?.IsAuthenticated != true)
    {
        return Results.Unauthorized();
    }

    var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
    var googleId =
        context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? context.User.FindFirst("sub")?.Value;

    if (string.IsNullOrWhiteSpace(email) ||
        string.IsNullOrWhiteSpace(googleId))
    {
        return Results.BadRequest(
            "Google did not return the required account details.");
    }

    if (email.Length > 45)
    {
        return Results.BadRequest(
            "The Google email is longer than the database allows.");
    }

    if (googleId.Length > 50)
    {
        return Results.BadRequest(
            "The Google account ID is longer than the database allows.");
    }

    var existingLogin = await db.UserLogins
        .FirstOrDefaultAsync(user => user.SocialUserId == googleId);

    if (existingLogin is not null)
    {
        return Results.Redirect("https://localhost:7022/dashboard");
    }

    await using var transaction =
        await db.Database.BeginTransactionAsync();

    try
    {
        var now = DateTime.Now;
        var name = context.User.Identity?.Name ?? email;

        var portalUser = new AskpearlsPortalUserList
        {
            RowViewTokenId = Guid.NewGuid(),
            Email = email,
            EmailAddress = email,
            FirstName = name,
            LoginType = "Google"
        };

        db.AskpearlsPortalUserLists.Add(portalUser);
        await db.SaveChangesAsync();

        var login = new UserLogin
        {
            UserId = checked((int)portalUser.UserId),
            FirstName = name,
            MailId = email,
            RegistrationDate = DateOnly.FromDateTime(now),
            RegistrationTime = TimeOnly.FromDateTime(now),
            Status = 2,
            LoginType = "Google",
            SocialUserId = googleId,
            SocialUserName = name
        };

        db.UserLogins.Add(login);

        var account = new AskpearlsPortalUserAccountList
        {
            RowViewTokenId = Guid.NewGuid(),
            OwnerUserTokenId = portalUser.UserId.ToString(),
            AccountType = "personal",
            AccountIsEnable = true,
            AccountIsVisible = true,
            IsPersonal = true,
            IsDating = false,
            IsBusiness = false,
            IsActive = true
        };

        db.AskpearlsPortalUserAccountLists.Add(account);

        await db.SaveChangesAsync();
        await transaction.CommitAsync();

        return Results.Redirect("https://localhost:7022/dashboard");
    }
    catch
    {
        await transaction.RollbackAsync();

        return Results.Problem(
            "Google sign-in could not save the account.");
    }
});

app.MapGet("/api/auth/me", async (
    ClaimsPrincipal user,
    ICurrentUser_Service currentUserService,
    CancellationToken cancellationToken) =>
{
    var googleId =
        user.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? user.FindFirst("sub")?.Value;

    var email = user.FindFirst(ClaimTypes.Email)?.Value;

    var profile = await currentUserService.GetCurrentUserAsync(
        googleId,
        email,
        cancellationToken);

    return profile is null
        ? Results.Unauthorized()
        : Results.Ok(profile);
})
.RequireAuthorization();

app.Run();