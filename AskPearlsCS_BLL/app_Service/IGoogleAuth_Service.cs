namespace AskPearlsCS_BLL.app_Service;

public interface IGoogleAuth_Service
{
    Task<GoogleAuthResult?> ValidateAndProvisionAsync(
        string idToken,
        string googleClientId,
        CancellationToken cancellationToken);
}

public sealed record GoogleAuthResult(
    long UserId,
    string Email,
    string? Name,
    bool IsNewUser);