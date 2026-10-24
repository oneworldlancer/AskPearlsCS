using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;


using AskPearlsCS_BLL.DecisionOSManager;
using AskPearlsCS_BLL.WidgetDecision;
using AskPearlsCS_BLL.app_Service;

using Microsoft.EntityFrameworkCore;
using AskPearlsCS_DAL.DBModels;

using AskPearlsCS_DAL.app_Repository;
using AskPearlsCS_DAL.DBContext;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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



// Recommendation layer
builder.Services.AddScoped<IRecommendationItem_Repository, RecommendationItem_Repository>();

builder.Services.AddScoped<IRecommendationItem_Service, RecommendationItem_Service>();



builder.Services.AddScoped<ICurrentUser_Repository, CurrentUser_Repository>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();






builder.Services.AddDbContext<DBContextModel>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "ASKPEARLSDB"));
});



builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
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
    });



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

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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
    
    //var name = context.User.Identity.Name ?? email;

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

    var name = context.User.Identity?.Name ?? email ?? "AskPearls User";

    var existingLogin = await db.UserLogins
        .FirstOrDefaultAsync(u => u.SocialUserId == googleId);

    if (existingLogin is not null)
    {
        //return Results.Ok(new
        //{
        //    message = "Google sign-in succeeded.",
        //    userId = existingLogin.UserId,
        //    email = existingLogin.MailId
        //});

        return Results.Redirect("https://localhost:7022/dashboard");
    }

    await using var transaction =
        await db.Database.BeginTransactionAsync();

    try
    {
        var now = DateTime.Now;

        // UserId is generated by the database identity column.
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

        // UserLogin.UserId is not an identity column, so use the
        // UserId generated for AskpearlsPortalUserList.
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

        // UaccountId is generated by its database identity column.
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

        ////////return Results.Ok(new
        ////////{
        ////////    message = "Google signup succeeded.",
        ////////    userId = portalUser.UserId,
        ////////    email
        ////////});
        ///
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
    ICurrentUserService currentUserService,
    CancellationToken cancellationToken) =>
{
    var googleId =
        user.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? user.FindFirst("sub")?.Value;

    var email = user.FindFirst(ClaimTypes.Email)?.Value;

    var profile = await currentUserService.GetCurrentUserAsync(
        googleId, email, cancellationToken);

    return profile is null
        ? Results.Unauthorized()
        : Results.Ok(profile);
})
.RequireAuthorization();


app.Run();