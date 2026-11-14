using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Text;

namespace AskPearlsCS_DOMAIN.app_Service
{

    public static class AskPearlsClaimTypes
    {
        public const string ASKPEARLS_USER_ID = "ASKPEARLS_USER_ID";
        public const string ASKPEARLS_USER_FIRST_NAME = "ASKPEARLS_USER_FIRST_NAME";
        public const string ASKPEARLS_USER_LAST_NAME = "ASKPEARLS_USER_LAST_NAME";
        public const string ASKPEARLS_USER_FULL_NAME = "ASKPEARLS_USER_FULL_NAME";
        public const string ASKPEARLS_USER_EMAIL = "ASKPEARLS_USER_EMAIL";
    }

    public interface ICurrentUserContext_Service
    {
        bool IsAuthenticated { get; }
        //long? UserId { get; }
        string? OwnerUserTokenID { get; }
          string? OwnerFirstName { get; }
        string? OwnerLastName { get; }
        string? OwnerFullName { get; }
      string? OwnerEmailAddress { get; }
        //long RequireUserId();
        string RequireOwnerUserTokenID();
        string? GetClaim(string claimType);

    }

    public sealed class CurrentUserContext_Service : ICurrentUserContext_Service
    {
        private readonly IHttpContextAccessor _accessor;
      
        public CurrentUserContext_Service(IHttpContextAccessor accessor) => _accessor = accessor;
      
        private ClaimsPrincipal? Principal => _accessor.HttpContext?.User;
      
        public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;
      
        public string? GetClaim(string claimType) => IsAuthenticated ? Principal?.FindFirst(claimType)?.Value : null;
     
        public string? OwnerUserTokenID => GetClaim(AskPearlsClaimTypes.ASKPEARLS_USER_ID);
        public string? OwnerFirstName => GetClaim(AskPearlsClaimTypes.ASKPEARLS_USER_FIRST_NAME);
        public string? OwnerLastName => GetClaim(AskPearlsClaimTypes.ASKPEARLS_USER_LAST_NAME);
        public string? OwnerFullName => GetClaim(AskPearlsClaimTypes.ASKPEARLS_USER_FULL_NAME);
        public string? OwnerEmailAddress => GetClaim(AskPearlsClaimTypes.ASKPEARLS_USER_EMAIL);
       
      //public string? UserId => (GetClaim(AskPearlsClaimTypes.ASKPEARLS_USER_ID), NumberStyles.None,
      //      CultureInfo.InvariantCulture, out var id) && id > 0 ? id : null;
       
      //  public string? OwnerUserTokenID => UserId?.ToString(CultureInfo.InvariantCulture);
        //public string RequireUserId() => OwnerUserTokenID ?? throw new UnauthorizedAccessException("Missing valid authenticated AskPearlsUserId claim.");
        public string RequireOwnerUserTokenID() => OwnerUserTokenID ?? throw new UnauthorizedAccessException("Missing valid authenticated AskPearlsUserId claim.");
        public string RequireOwnerFirstName() => OwnerFirstName ?? throw new UnauthorizedAccessException("Missing valid authenticated AskPearlsFirstName claim.");
        public string RequireOwnerLastName() => OwnerLastName ?? throw new UnauthorizedAccessException("Missing valid authenticated AskPearlsLastName claim.");
        public string RequireOwnerFullName() => OwnerFullName ?? throw new UnauthorizedAccessException("Missing valid authenticated AskPearlsFullName claim.");
        public string RequireOwnerEmailAddress() => OwnerEmailAddress ?? throw new UnauthorizedAccessException("Missing valid authenticated AskPearlsEmailAddress claim.");
   
    
    }

}
