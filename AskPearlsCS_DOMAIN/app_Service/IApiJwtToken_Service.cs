using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace AskPearlsCS_DOMAIN.app_Service
{
   
    public interface IApiJwtToken_Service
    {
        string CreateForCurrentUser();
    }

    public sealed class ApiJwtToken_Service : IApiJwtToken_Service
    {
        private readonly ICurrentUserContext_Service _user;
        private readonly IConfiguration _configuration;
        public ApiJwtToken_Service(ICurrentUserContext_Service user, IConfiguration configuration)
        {
            _user = user;
            _configuration = configuration;
        }

        public string CreateForCurrentUser()
        {
            var id = _user.RequireUserId().ToString(CultureInfo.InvariantCulture);
            var section = _configuration.GetSection("ApiJwt");
            var issuer = section["Issuer"];
            var audience = section["Audience"];
            var encodedKey = section["SigningKey"];
            if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience) || string.IsNullOrWhiteSpace(encodedKey))
                throw new InvalidOperationException("ApiJwt issuer, audience or signing key is missing.");
            byte[] key;
            try { key = Convert.FromBase64String(encodedKey); }
            catch (FormatException ex) { throw new InvalidOperationException("ApiJwt signing key must be Base64.", ex); }
            if (key.Length < 32) throw new InvalidOperationException("ApiJwt signing key must decode to at least 32 bytes.");
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, id),
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(AskPearlsClaimTypes.ASKPEARLS_USER_ID, id)
                },
                notBefore: now,
                expires: now.AddMinutes(3),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }


}
