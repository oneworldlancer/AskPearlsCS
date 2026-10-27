using AskPearlsCS_DOMAIN.app_Identity;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;

using AskPearlsCS_PORTAL.app_Utility;



var builder = WebApplication.CreateBuilder(args);




#region Identity

builder.Services.AddDbContext<AppCore_DBContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ASKPEARLSDB")));

builder.Services
    .AddIdentity<AppCore_IdentityUser, AppCore_IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppCore_DBContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".AskPearls.Portal.Auth";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(600);
    options.SlidingExpiration = true;
});


#endregion


builder.Services.AddScoped<ApiAccessTokenService>();

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});




builder.Services
    .AddAuthentication()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["GoogleOAuth:ClientId"]!;
    options.ClientSecret = builder.Configuration["GoogleOAuth:ClientSecret"]!;
    options.SignInScheme = IdentityConstants.ExternalScheme;
    options.SaveTokens = true;

    options.Events.OnCreatingTicket = context =>
    {
        var response = context.TokenResponse.Response;

        if (response is not null &&
            response.RootElement.TryGetProperty("id_token", out var tokenElement))
        {
            var idToken = tokenElement.GetString();

            if (!string.IsNullOrWhiteSpace(idToken))
            {
                var tokens = context.Properties.GetTokens().ToList();
                tokens.RemoveAll(token => token.Name == "id_token");
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


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Home/Index";
});


// HTTP client for communicating with the AskPearls API.
builder.Services.AddHttpClient();


// 1. MVC
builder.Services.AddControllersWithViews();

//////////// 2. Database
//////////builder.Services.AddDbContext<DBContextModel>(options =>
//////////    options.UseSqlServer(
//////////        builder.Configuration.GetConnectionString("ASKPEARLSDB")));

// 3. Session support for iOwnerModel
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".AskPearls.Portal.Session";
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();



app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
