using Identity.Domain.Interfaces.Identity;
using Identity.Domain.Models;
using Identity.Infraestructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.Infrastructure.Identity
{
    public class UserIdentity : IUserIdentity
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly string _secret;
        private readonly int _expiryMinutes;

        public UserIdentity(IHttpContextAccessor httpContextAccessor,
            IOptions<AccessTokenConfiguration> options)
        {
            _httpContextAccessor = httpContextAccessor; 

            _secret = options.Value.Secret;
            _expiryMinutes = options.Value.ExpiryMinutes;
        }

        public async Task<AccessToken> CreateTokenByUserIdAsync(string userId)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                };

                var expiryMinutes = (double)_expiryMinutes;
                var expires = DateTime.Now.AddMinutes(expiryMinutes);
                var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
                var signingCredentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
                
                var jwt = new JwtSecurityToken(
                    claims: claims,
                    expires: expires,
                    signingCredentials: signingCredentials
                );

                var accessKey = new JwtSecurityTokenHandler().WriteToken(jwt);
                var validTo = jwt.ValidTo;

                return await Task.FromResult(new AccessToken
                {
                    Token = JwtBearerDefaults.AuthenticationScheme + " " + accessKey,
                    Expires = validTo
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetUserIdFromToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

            if (token.StartsWith("Bearer "))
                token = token.Substring(7);
            else
                return null;

            var jwtToken = tokenHandler.ReadJwtToken(token);
            var claim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (claim == null)
                return null;

            return claim.Value;
        }
    }
}
