using AskPearlsCS_DAL.DBContext;
using AskPearlsCS_DOMAIN.app_ContractsManager;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
namespace AskPearlsCS_DAL.app_Repository;
public interface IPaddle_Repository
{
    Task<string?> PriceAsync(string environment,string plan,string interval,CancellationToken ct);
    Task ReserveAsync(Guid id,string environment,string owner,string price,CancellationToken ct);
    Task AttachAsync(Guid id,string transaction,CancellationToken ct);
    Task StoreAsync(string environment,PaddleEvent evt,CancellationToken ct);
}
public sealed class Paddle_Repository(DBContextModel db) : IPaddle_Repository
{
    private async Task<object?> Execute(string sql,CancellationToken ct,params (string Name,object Value)[] args)
    {
        var connection=db.Database.GetDbConnection();
        var close=connection.State == System.Data.ConnectionState.Closed;
        if(close) await connection.OpenAsync(ct);
        try {
            await using var command=connection.CreateCommand(); command.CommandText=sql;
            foreach(var arg in args){var p=command.CreateParameter();p.ParameterName=arg.Name;p.Value=arg.Value;command.Parameters.Add(p);}
            return await command.ExecuteScalarAsync(ct);
        } finally { if(close) await connection.CloseAsync(); }
    }
    public async Task<string?> PriceAsync(string environment,string plan,string interval,CancellationToken ct) =>
        (await Execute("SELECT PriceId FROM dbo.ASKPEARLS_PORTAL_APPLICATION_SUBSCRIPTION_PLAN_PRICE_LIST WHERE Environment=@env AND PlanCode=@plan AND BillingInterval=@interval AND IsActive=1",ct,("@env",environment),("@plan",plan),("@interval",interval))) as string;
    public async Task ReserveAsync(Guid id,string environment,string owner,string price,CancellationToken ct) =>
        _ = await Execute("INSERT dbo.ASKPEARLS_PORTAL_BILLING_CHECKOUT_LIST(CheckoutId,Environment,OwnerToken,PriceId) VALUES(@id,@env,@owner,@price)",ct,("@id",id),("@env",environment),("@owner",owner),("@price",price));
    public async Task AttachAsync(Guid id,string transaction,CancellationToken ct) =>
        _ = await Execute("UPDATE dbo.ASKPEARLS_PORTAL_BILLING_CHECKOUT_LIST SET TransactionId=@txn WHERE CheckoutId=@id",ct,("@txn",transaction),("@id",id));
    public async Task StoreAsync(string environment,PaddleEvent evt,CancellationToken ct) =>
        _ = await Execute("BEGIN TRY INSERT dbo.ASKPEARLS_PORTAL_BILLING_WEBHOOK_EVENT_LIST(Environment,EventId,EventType,OccurredUtc,Payload) VALUES(@env,@id,@type,@at,@payload); END TRY BEGIN CATCH IF ERROR_NUMBER() NOT IN (2601,2627) THROW; END CATCH",ct,("@env",environment),("@id",evt.EventId),("@type",evt.EventType),("@at",evt.OccurredUtc),("@payload",evt.Payload));
}
