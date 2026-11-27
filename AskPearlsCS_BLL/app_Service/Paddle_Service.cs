using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AskPearlsCS_DAL.app_Repository;
using AskPearlsCS_DOMAIN.app_ContractsManager;
using Microsoft.Extensions.Options;
namespace AskPearlsCS_BLL.app_Service;
public sealed class Paddle_Service(IPaddle_Repository repository,HttpClient client,IOptions<PaddleOptions> options)
{
    private PaddleOptions Settings => options.Value;
    public async Task<PaddleCheckoutResult> CheckoutAsync(string owner,PaddleCheckoutRequest request,CancellationToken ct)
    {
        if(!Settings.Enabled || Settings.Environment != "Sandbox" || string.IsNullOrWhiteSpace(Settings.ApiKey) || string.IsNullOrWhiteSpace(Settings.ClientSideToken)) throw new InvalidOperationException("Sandbox checkout disabled.");
        if(string.IsNullOrWhiteSpace(request.PlanCode) || request.PlanCode.Length>64 || request.BillingInterval is not ("month" or "year")) throw new ArgumentException("Invalid plan.");
        var price=await repository.PriceAsync(Settings.Environment,request.PlanCode,request.BillingInterval,ct);
        if(price is null || !price.StartsWith("pri_")) throw new ArgumentException("Plan unavailable.");
        var id=Guid.NewGuid(); await repository.ReserveAsync(id,Settings.Environment,owner,price,ct);
        using var message=new HttpRequestMessage(HttpMethod.Post,"https://sandbox-api.paddle.com/transactions");
        message.Headers.Authorization=new AuthenticationHeaderValue("Bearer",Settings.ApiKey);
        message.Content=JsonContent.Create(new {items=new[]{new {price_id=price,quantity=1}},collection_mode="automatic",custom_data=new {askpearls_checkout_id=id.ToString("D")}});
        using var response=await client.SendAsync(message,ct); response.EnsureSuccessStatusCode();
        using var result=JsonDocument.Parse(await response.Content.ReadAsStringAsync(ct));
        var txn=result.RootElement.GetProperty("data").GetProperty("id").GetString() ?? throw new InvalidOperationException("Missing transaction.");
        await repository.AttachAsync(id,txn,ct);
        return new(txn,Settings.ClientSideToken,Settings.Environment);
    }
    public bool Verify(byte[] body,string header)
    {
        if(Settings.Environment!="Sandbox" || string.IsNullOrWhiteSpace(Settings.WebhookSecret)) return false;
        var parts=header.Split(';').Select(x=>x.Trim().Split('=',2)).Where(x=>x.Length==2).ToArray();
        var ts=parts.FirstOrDefault(x=>x[0]=="ts")?[1];
        if(!long.TryParse(ts,out var stamp) || Math.Abs((decimal)DateTimeOffset.UtcNow.ToUnixTimeSeconds()-stamp)>5) return false;
        var prefix=Encoding.UTF8.GetBytes(ts+":"); var data=new byte[prefix.Length+body.Length]; prefix.CopyTo(data,0);body.CopyTo(data,prefix.Length);
        var hash=HMACSHA256.HashData(Encoding.UTF8.GetBytes(Settings.WebhookSecret),data);
        foreach(var part in parts.Where(x=>x[0]=="h1")){try{if(CryptographicOperations.FixedTimeEquals(hash,Convert.FromHexString(part[1])))return true;}catch(FormatException){}}
        return false;
    }
    public async Task ReceiveAsync(byte[] body,CancellationToken ct)
    {
        using var json=JsonDocument.Parse(body);var root=json.RootElement;
        var evt=new PaddleEvent(root.GetProperty("event_id").GetString()!,root.GetProperty("event_type").GetString()!,root.GetProperty("occurred_at").GetDateTimeOffset(),Encoding.UTF8.GetString(body));
        await repository.StoreAsync(Settings.Environment,evt,ct);
    }
}
