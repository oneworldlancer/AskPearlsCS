using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_BLL.app_Service
{

    public interface ICurrentUser_Service
    {
        Task<CurrentUserDto?> GetCurrentUserAsync(
            string? googleId,
            string? email,
            CancellationToken cancellationToken);
    }

    public sealed record CurrentUserDto(
        string OwnerUserTokenID,
        string? FirstName,
        string? LastName,
        string EmailAddress);
    
    //,
    //    long? AccountId,
    //    string? AccountType
}
