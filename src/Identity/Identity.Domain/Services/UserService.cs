using Identity.Domain.Core;
using Identity.Domain.Interfaces.Identity;
using Identity.Domain.Interfaces.Repositories;
using Identity.Domain.Interfaces.Services;
using Identity.Domain.Models;

namespace Identity.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly NotificationContext _notification;
        private readonly IUserRepository _repository;
        private readonly IUserIdentity _identity;

        public UserService(NotificationContext notification,
            IUserRepository repository,
            IUserIdentity identity)
        {
            _notification = notification;
            _repository = repository;
            _identity = identity;
        }       

        public async Task<User> GetByIdAsync(string id)
        {
            var user = await _repository.GetByIdAsync(id);

            if (user == null) 
            {
                _notification.AddNotification("Não foi encontrado nenhum Usuário");

                return null;
            }

            return user;
        }

        public async Task<User> GetAsync()
        {
            var userId = _identity.GetUserIdFromToken();

            if (string.IsNullOrEmpty(userId))
            {
                _notification.AddNotification("Token Inválido");

                return null;
            }

            var user = await _repository.GetByIdAsync(userId);

            if (user == null)
            {
                _notification.AddNotification("Não foi encontrado nenhum Usuário");

                return null;
            }

            return user;
        }

        public async Task InsertAsync(User user)
        {
            await _repository.InsertAsync(user);
        }

        public async Task UpdateAsync(User user)
        {
            await _repository.UpdateAsync(user);
        }

        public async Task DeleteAsync()
        {
            var userId = _identity.GetUserIdFromToken();

            if (string.IsNullOrEmpty(userId))
            {
                _notification.AddNotification("Token Inválido");

                return;
            }

            var user = await _repository.GetByIdAsync(userId);

            if (user == null) 
            {
                _notification.AddNotification("Não foi possivel encontrar nenhum Usuário");

                return;
            }

            await _repository.DeleteAsync(user);
        }
    }
}
