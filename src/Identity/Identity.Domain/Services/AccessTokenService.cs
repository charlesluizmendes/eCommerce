using Identity.Domain.Core;
using Identity.Domain.Interfaces.Identity;
using Identity.Domain.Interfaces.Repositories;
using Identity.Domain.Interfaces.Services;
using Identity.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Services
{
    public class AccessTokenService : IAccessTokenService
    {
        private readonly NotificationContext _notification;
        private readonly IUserIdentity _identity;
        private readonly IUserRepository _userRepository;

        public AccessTokenService(NotificationContext notification,
            IUserIdentity identity,
            IUserRepository userRepository)
        {
            _notification = notification;   
            _identity = identity;
            _userRepository = userRepository;
        }

        public async Task<AccessToken> CreateAcessTokenByUserAsync(User user)
        {
            var _user = await _userRepository.GetUserByEmailAsync(user.Email);

            if (_user == null) 
            {
                _notification.AddNotification("Usuário incorreto");

                return null;
            }

            var result = VerifyHashedPassword(_user, _user.PasswordHash, user.PasswordHash);

            if (result)
                return await _identity.CreateTokenByUserIdAsync(_user.Id);
            else
            {
                _notification.AddNotification("Senha incorreta");

                return null;
            }
        }

        #region private Methods

        private static bool VerifyHashedPassword(User user, string passwordHash, string password)
        {
            var hasher = new PasswordHasher<User>();

            if (passwordHash == null && password == null)
                return false;

            var verify = hasher.VerifyHashedPassword(user, passwordHash, password);

            if (verify == PasswordVerificationResult.Success)
                return true;
            else
                return false;
        }

        #endregion
    }
}
