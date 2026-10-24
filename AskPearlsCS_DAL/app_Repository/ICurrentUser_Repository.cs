using AskPearlsCS_DAL.DBModels;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_DAL.app_Repository
{

    public interface ICurrentUser_Repository
    {
        Task<UserLogin?> GetLoginByGoogleIdAsync(
            string googleId, CancellationToken cancellationToken);

        Task<UserLogin?> GetLoginByEmailAsync(
            string email, CancellationToken cancellationToken);

        Task<AskpearlsPortalUserList?> GetPortalUserByIdAsync(
            long userId, CancellationToken cancellationToken);

        Task<AskpearlsPortalUserAccountList?> GetAccountByOwnerUserTokenIdAsync(
            string ownerUserTokenId, CancellationToken cancellationToken);
    }
}
