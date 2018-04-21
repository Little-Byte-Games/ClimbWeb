using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Climb.Services.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace Climb.Services
{
    public class TokenHelper : ITokenHelper
    {
        private const string TokenPrefix = "Bearer ";

        private readonly JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        private readonly IApplicationUserRepository userRepository;

        public TokenHelper(IApplicationUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public string CreateUserToken(SecurityKey securityKey, DateTime expires, string email)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email)
            };

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "climb.com",
                audience: "climb",
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

            var serializedToken = new JwtSecurityTokenHandler().WriteToken(token);
            serializedToken = $"Bearer {serializedToken}";

            return serializedToken;
        }

        public async Task<string> GetAuthorizedUserID(string token)
        {
            if(token.StartsWith(TokenPrefix))
            {
                token = token.Substring(TokenPrefix.Length);
            }

            try
            {
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var emailclaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                if(emailclaim == null)
                {
                    return string.Empty;
                }

                var email = emailclaim.Value;
                var user = await userRepository.GetByEmail(email);
                return user?.Id;
            }
            catch(ArgumentException)
            {
                return string.Empty;
            }
        }
    }
}