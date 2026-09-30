using AskPearlsCS_BLL.DecisionOSManager;
using AskPearlsCS_BLL.WidgetDecision;

using AskPearlsCS_DAL.DBContext;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<IDecisionEngine, DecisionEngine>();

builder.Services.AddScoped<    IWidgetDecisionService,    WidgetDecisionService>();



builder.Services.AddDbContext<DBContextModel>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "ASKPEARLSDB"));
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseCors("Widget");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
