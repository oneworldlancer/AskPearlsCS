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
        public const string ASKPEARLS_USER_NAME = "ASKPEARLS_USER_NAME";
        public const string ASKPEARLS_USER_EMAIL = "ASKPEARLS_USER_EMAIL";
    }

    public interface ICurrentUserContext_Service
    {
        bool IsAuthenticated { get; }
        long? UserId { get; }
        string? OwnerUserTokenId { get; }
        long RequireUserId();
        string RequireOwnerUserTokenId();
        string? GetClaim(string claimType);
    }

    public sealed class CurrentUserContext_Service : ICurrentUserContext_Service
    {
        private readonly IHttpContextAccessor _accessor;
        public CurrentUserContext_Service(IHttpContextAccessor accessor) => _accessor = accessor;
        private ClaimsPrincipal? Principal => _accessor.HttpContext?.User;
        public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;
        public string? GetClaim(string claimType) => IsAuthenticated ? Principal?.FindFirst(claimType)?.Value : null;
        public long? UserId => long.TryParse(GetClaim(AskPearlsClaimTypes.ASKPEARLS_USER_ID), NumberStyles.None,
            CultureInfo.InvariantCulture, out var id) && id > 0 ? id : null;
        public string? OwnerUserTokenId => UserId?.ToString(CultureInfo.InvariantCulture);
        public long RequireUserId() => UserId ?? throw new UnauthorizedAccessException("Missing valid authenticated AskPearlsUserId claim.");
        public string RequireOwnerUserTokenId() => OwnerUserTokenId ?? throw new UnauthorizedAccessException("Missing valid authenticated AskPearlsUserId claim.");
    }

}
