using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ForumSugar.Models.Entities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace ForumSugar.Helpers
{
    public static class JwtHelper
    {
        private static readonly string SecretKey = "A9!k72#Bd4$XzP0q!Lm5v@N1oR3e*UyCw6^GTzJh8&WsdMb9#A0VxZ%fE7HyKtLpmanh"; // Nên đưa vào appsettings
        private static readonly string Issuer = "ForumSugarAPI"; // Phải trùng với ValidIssuer
        private static readonly int ExpiryInDays = 7;

        public static string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);
        

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {  
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("IsLocked", user.IsLocked.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(ExpiryInDays),
                Issuer = Issuer, 
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
