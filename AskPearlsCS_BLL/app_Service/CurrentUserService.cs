using AskPearlsCS_DAL.app_Repository;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_BLL.app_Service
{

    public sealed class CurrentUserService : ICurrentUserService
    {
        private readonly ICurrentUser_Repository _repository;

        public CurrentUserService(ICurrentUser_Repository repository)
        {
            _repository = repository;
        }

        public async Task<CurrentUserDto?> GetCurrentUserAsync(
            string? googleId,
            string? email,
            CancellationToken cancellationToken)
        {
            var login = !string.IsNullOrWhiteSpace(googleId)
                ? await _repository.GetLoginByGoogleIdAsync(
                    googleId, cancellationToken)
                : null;

            login ??= !string.IsNullOrWhiteSpace(email)
                ? await _repository.GetLoginByEmailAsync(
                    email, cancellationToken)
                : null;

            if (login is null)
                return null;

            var portalUser = await _repository.GetPortalUserByIdAsync(
                login.UserId, cancellationToken);

            var account = await _repository.GetAccountByOwnerUserTokenIdAsync(
                login.UserId.ToString(), cancellationToken);

            return new CurrentUserDto(
                portalUser?.UserId ?? login.UserId,
                portalUser?.FirstName ?? login.FirstName,
                login.MailId,
                account?.UaccountId,
                account?.AccountType);
        }
    }

}