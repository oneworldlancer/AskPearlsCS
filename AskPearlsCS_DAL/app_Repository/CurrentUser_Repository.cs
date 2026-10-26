using AskPearlsCS_DAL.DBContext;
using AskPearlsCS_DAL.DBModels;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_DAL.app_Repository
{

    public sealed class CurrentUser_Repository : ICurrentUser_Repository
    {
        private readonly DBContextModel _db;

        public CurrentUser_Repository(DBContextModel db)
        {
            _db = db;
        }

        public Task<UserLogin?> GetLoginByGoogleIdAsync(
            string googleId, CancellationToken cancellationToken)
        {
            return _db.UserLogins.FirstOrDefaultAsync(
                x => x.SocialUserId == googleId, cancellationToken);
        }

        public Task<UserLogin?> GetLoginByEmailAsync(
            string email, CancellationToken cancellationToken)
        {
            return _db.UserLogins.FirstOrDefaultAsync(
                x => x.MailId == email, cancellationToken);
        }

        public Task<AskpearlsPortalUserList?> GetPortalUserByIdAsync(
            long userId, CancellationToken cancellationToken)
        {
            return _db.AskpearlsPortalUserLists.FirstOrDefaultAsync(
                x => x.UserId == userId, cancellationToken);
        }

        public Task<AskpearlsPortalUserAccountList?>
            GetAccountByOwnerUserTokenIdAsync(
                string ownerUserTokenId,
                CancellationToken cancellationToken)
        {
            return _db.AskpearlsPortalUserAccountLists.FirstOrDefaultAsync(
                x => x.OwnerUserTokenId == ownerUserTokenId,
                cancellationToken);
        }
    }

}