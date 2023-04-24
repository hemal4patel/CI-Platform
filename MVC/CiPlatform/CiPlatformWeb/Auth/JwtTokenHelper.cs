using CiPlatformWeb.Entities.Auth;
using CiPlatformWeb.Entities.DataModels;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace CiPlatformWeb.Auth
{
    public class JwtTokenHelper
    {
        public static string GenerateToken (JwtSetting jwtSetting, User sessionUser)
        {
            if (jwtSetting == null)
                return string.Empty;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, sessionUser.FirstName),
                new Claim(ClaimTypes.NameIdentifier, sessionUser.Email),
                new Claim(ClaimTypes.Role, sessionUser.Role),
                new Claim("CustomClaimForUser", JsonSerializer.Serialize(sessionUser))  
            };

            var token = new JwtSecurityToken(
                jwtSetting.Issuer,
                jwtSetting.Audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(20), 
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
