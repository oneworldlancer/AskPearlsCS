using AskPearlsCS_DAL.DBContext;
using AskPearlsCS_DAL.DBModels;

using Microsoft.EntityFrameworkCore;

namespace AskPearlsCS_DAL.app_Repository;

public sealed class GoogleAuth_Repository : IGoogleAuth_Repository
{
    private readonly DBContextModel _db;

    public GoogleAuth_Repository(DBContextModel db)
    {
        _db = db;
    }

    public Task<UserLogin?> GetByGoogleIdAsync(
        string googleId, CancellationToken cancellationToken) =>
        _db.UserLogins.FirstOrDefaultAsync(
            x => x.SocialUserId == googleId, cancellationToken);

    public Task<UserLogin?> GetByEmailAsync(
        string email, CancellationToken cancellationToken) =>
        _db.UserLogins.FirstOrDefaultAsync(
            x => x.MailId == email, cancellationToken);

    public async Task<UserLogin> CreateGoogleUserAsync(
        string googleId,
        string email,
        string name,
        CancellationToken cancellationToken)
    {
        await using var transaction =
            await _db.Database.BeginTransactionAsync(cancellationToken);

        var now = DateTime.Now;
        var safeName = name.Length > 100 ? name[..100] : name;

        var portalUser = new AskpearlsPortalUserList
        {
            RowViewTokenId = Guid.NewGuid(),
            Email = email,
            EmailAddress = email,
            FirstName = safeName,
            LoginType = "Google"
        };

        _db.AskpearlsPortalUserLists.Add(portalUser);
        await _db.SaveChangesAsync(cancellationToken);

        var login = new UserLogin
        {
            UserId = checked((int)portalUser.UserId),
            FirstName = safeName,
            MailId = email,
            RegistrationDate = DateOnly.FromDateTime(now),
            RegistrationTime = TimeOnly.FromDateTime(now),
            Status = 2,
            LoginType = "Google",
            SocialUserId = googleId,
            SocialUserName = name.Length > 50 ? name[..50] : name
        };

        var account = new AskpearlsPortalUserAccountList
        {
            RowViewTokenId = Guid.NewGuid(),
            OwnerUserTokenId = portalUser.UserId.ToString(),
            AccountType = "personal",
            AccountIsEnable = true,
            AccountIsVisible = true,
            IsPersonal = true,
            IsDating = false,
            IsBusiness = false,
            IsActive = true
        };

        _db.UserLogins.Add(login);
        _db.AskpearlsPortalUserAccountLists.Add(account);

        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return login;
    }

    public async Task<UserLogin> LinkGoogleIdAsync(
        UserLogin login,
        string googleId,
        string name,
        CancellationToken cancellationToken)
    {
        login.SocialUserId = googleId;
        login.SocialUserName = name.Length > 50 ? name[..50] : name;
        login.LoginType = "Google";

        await _db.SaveChangesAsync(cancellationToken);
        return login;
    }
}