using Payment.Domain.Interfaces.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Payment.Infrastructure.Identity
{
    public class UserIdentity : IUserIdentity
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserIdentity(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserIdFromToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

            if (token.StartsWith("Bearer "))
            {
                token = token.Substring(7);
            }

            var jwtToken = tokenHandler.ReadJwtToken(token);
            var claim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            return claim.Value;
        }
    }
}
