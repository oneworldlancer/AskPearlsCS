using Microsoft.Extensions.Options;

using System.Net;
using System.Net.Mail;

using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
namespace AskPearlsCS_PORTAL.app_Manager;


public sealed class SupportSmtpOptions
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string FromAddress { get; set; } = "";
    public string ToAddress { get; set; } = "HelpDesk@AskPearls.com";
}

//public interface ISupportEmailService
//{
//    Task SendAsync(string name, string email, string message, string? reference, CancellationToken ct);
//}

public sealed class SupportEmailService : ISupportEmailService
{
    private readonly SupportSmtpOptions _settings;
    public SupportEmailService(IOptions<SupportSmtpOptions> settings) => _settings = settings.Value;

    public async Task SendAsync(string name, string email, string message, string? reference, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_settings.Host) || string.IsNullOrWhiteSpace(_settings.FromAddress))
            throw new InvalidOperationException("SMTP settings are not configured.");
        using var mail = new MailMessage
        {
            From = new MailAddress(_settings.FromAddress, "AskPearls Support Form"),
            Subject = string.IsNullOrWhiteSpace(reference) ? "[AskPearls Support] New request" : "[AskPearls Support] Notification enquiry",
            Body = $"Name: {name}\r\nEmail: {email}\r\nReference: {reference ?? "None"}\r\n\r\nMessage:\r\n{message}",
            IsBodyHtml = false
        };
        mail.To.Add(new MailAddress(_settings.ToAddress));
        mail.ReplyToList.Add(new MailAddress(email, name));
        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.EnableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            Timeout = 20000
        };
        ct.ThrowIfCancellationRequested();
        // SmtpClient does not reliably support cancellation; send runs asynchronously.
        await client.SendMailAsync(mail, ct);
    }
}
