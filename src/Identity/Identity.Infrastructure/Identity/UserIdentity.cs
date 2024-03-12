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
        private readonly string _iss;
        private readonly string _aud;

        public UserIdentity(IHttpContextAccessor httpContextAccessor,
            IOptions<AccessTokenConfiguration> audienceOptions)
        {
            _httpContextAccessor = httpContextAccessor; 
            _secret = audienceOptions.Value.Secret;
            _iss = audienceOptions.Value.Iss;
            _aud = audienceOptions.Value.Aud;
        }

        public async Task<AccessToken> CreateTokenByUserIdAsync(string userId)
        {
            try
            {
                var claims = new[]
                {
                     new Claim(ClaimTypes.NameIdentifier, userId),
                };

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_secret != null ? _secret : "")
                    );

                var creds = new SigningCredentials(
                    key, SecurityAlgorithms.HmacSha256
                    );

                var jwt = new JwtSecurityToken(
                    issuer: _iss,
                    audience: _aud,
                    expires: DateTime.Now.AddMinutes(Convert.ToInt32(30)),
                    claims: claims,
                    signingCredentials: creds
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
            {
                token = token.Substring(7);
            }

            var jwtToken = tokenHandler.ReadJwtToken(token);
            var claim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            return claim.Value;
        }
    }
}
