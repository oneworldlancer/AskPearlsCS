using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;



namespace AskPearlsCS_PORTAL.app_Utility
{


    public sealed class ApiAccessTokenService(IConfiguration configuration)
    {
        public string Create(string askPearlsUserId)
        {
            var settings = configuration.GetSection("ApiJwt");
            var key = settings["SigningKey"]
                ?? throw new InvalidOperationException("ApiJwt:SigningKey is missing.");

            var claims = new[]
            {
            new Claim("askpearls_user_id", askPearlsUserId)
        };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: settings["Issuer"],
                audience: settings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
