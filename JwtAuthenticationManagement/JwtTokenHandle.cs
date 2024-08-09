using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JwtAuthenticationManagement
{
    public class JwtTokenHandle
    {
        private const string secretKey = "ijurkbdlhmklqacwqzdxmkkhvqowlyqa99";
        private const string issuer = "localhost:7144";

        public async Task<string> GenerateJwtTokenTw(ClainmInfor account)
        {

            List<Claim> claims = new()
                {
                     new Claim(ClaimTypes.NameIdentifier, account.UserId + ""),
                };

            foreach (var role in account.Roles)
            {
                claims.Add(new Claim("scope", role.Name));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: issuer,
                audience: issuer,
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        public Guid VerifyToken(string token)
        {
        var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = issuer,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            try
            {
                // Validate token and get principal
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                // Get claims from principal
                var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                var roleClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

                if (userIdClaim == null)
                {
                    throw new SecurityTokenException("Invalid token");
                }
                var userId = Guid.Parse(userIdClaim.Value);
                return userId;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new SecurityTokenException("Token validation failed", ex);
            }
        }
    }

    public class ClainmInfor
    {
        public Guid UserId { get; set; }
        public List<RoleInfo> Roles { get; set; } = new List<RoleInfo>();
    }

    public class RoleInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = String.Empty;
    }
}
