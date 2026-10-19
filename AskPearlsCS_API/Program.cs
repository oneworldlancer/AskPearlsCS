using AskPearlsCS_BLL.DecisionOSManager;
using AskPearlsCS_BLL.WidgetDecision;
using AskPearlsCS_BLL.app_Service;

using AskPearlsCS_DAL.app_Repository;
using AskPearlsCS_DAL.DBContext;

using Microsoft.EntityFrameworkCore;

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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("Widget");
app.UseAuthorization();

app.MapControllers();

app.Run();