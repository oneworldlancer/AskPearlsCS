using AskPearlsCS_DAL.DBModels;

namespace AskPearlsCS_DAL.app_Repository;

public interface IGoogleAuth_Repository
{
    Task<UserLogin?> GetByGoogleIdAsync(
        string googleId, CancellationToken cancellationToken);

    Task<UserLogin?> GetByEmailAsync(
        string email, CancellationToken cancellationToken);

    Task<UserLogin> CreateGoogleUserAsync(
        string googleId,
        string email,
        string name,
        CancellationToken cancellationToken);

    Task<UserLogin> LinkGoogleIdAsync(
        UserLogin login,
        string googleId,
        string name,
        CancellationToken cancellationToken);
}