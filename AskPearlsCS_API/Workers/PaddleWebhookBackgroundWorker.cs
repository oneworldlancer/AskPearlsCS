using AskPearlsCS_BLL.app_Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace AskPearlsCS_API.Workers;
public sealed class PaddleWebhookBackgroundWorker(IServiceScopeFactory scopes, IConfiguration config, ILogger<PaddleWebhookBackgroundWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (config.GetValue<bool>("Paddle:WebhookWorkerEnabled") && config.GetValue<bool>("Paddle:IsolatedSandboxDatabase"))
                {
                    using var scope = scopes.CreateScope();
                    await scope.ServiceProvider.GetRequiredService<PaddleWebhook_Service>().ProcessOneAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception ex) { logger.LogError("Paddle worker failed: {ErrorType}", ex.GetType().Name); }
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
