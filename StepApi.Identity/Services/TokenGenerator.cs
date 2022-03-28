using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using StepApi.Identity.Model;
using StepApi.Identity.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace StepApi.Identity.Services
{
    public class TokenGenerator : ITokenGenerator
    {
        public string CreateJwtToken(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescription = new SecurityTokenDescriptor
            {                
                Expires = DateTime.Now.AddMinutes(AuthOptions.LIFETIME),
                Issuer = AuthOptions.ISSUER,
                Audience = AuthOptions.AUDIENCE,
                Subject = GetIdentity(user),
                SigningCredentials = new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescription);

            return tokenHandler.WriteToken(token);
        }

        public string CreateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        private ClaimsIdentity GetIdentity(AppUser user)
        {
            var claims = new List<Claim>()
            {
                 new Claim(ClaimTypes.Name,user.FullName),
                 new Claim(ClaimTypes.Email,user.Email),
                 new Claim(ClaimTypes.NameIdentifier,user.Id),
            };

            ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,ClaimsIdentity.DefaultRoleClaimType);


            return claimsIdentity;
        }
    }
}
