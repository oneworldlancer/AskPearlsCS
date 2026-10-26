using AskPearlsCS_DAL.app_Repository;

using Google.Apis.Auth;

namespace AskPearlsCS_BLL.app_Service;

public sealed class GoogleAuth_Service : IGoogleAuth_Service
{
    private readonly IGoogleAuth_Repository _repository;

    public GoogleAuth_Service(IGoogleAuth_Repository repository)
    {
        _repository = repository;
    }

    public async Task<GoogleAuthResult?> ValidateAndProvisionAsync(
        string idToken,
        string googleClientId,
        CancellationToken cancellationToken)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(
            idToken,
            new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { googleClientId }
            });

        if (string.IsNullOrWhiteSpace(payload.Subject) ||
            string.IsNullOrWhiteSpace(payload.Email) ||
            payload.EmailVerified != true)
        {
            return null;
        }

        var googleId = payload.Subject;
        var email = payload.Email.Trim().ToLowerInvariant();
        var name = string.IsNullOrWhiteSpace(payload.Name)
            ? email
            : payload.Name.Trim();

        if (email.Length > 45 || googleId.Length > 50)
            return null;

        var login = await _repository.GetByGoogleIdAsync(
            googleId, cancellationToken);

        if (login is not null)
        {
            return new GoogleAuthResult(
                login.UserId, login.MailId, login.FirstName, false);
        }

        login = await _repository.GetByEmailAsync(
            email, cancellationToken);

        if (login is not null)
        {
            if (!string.IsNullOrWhiteSpace(login.SocialUserId) &&
                login.SocialUserId != googleId)
            {
                throw new UnauthorizedAccessException(
                    "This email is already linked to another Google account.");
            }

            login = await _repository.LinkGoogleIdAsync(
                login, googleId, name, cancellationToken);

            return new GoogleAuthResult(
                login.UserId, login.MailId, login.FirstName, false);
        }

        login = await _repository.CreateGoogleUserAsync(
            googleId, email, name, cancellationToken);

        return new GoogleAuthResult(
            login.UserId, login.MailId, login.FirstName, true);
    }
}