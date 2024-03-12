using Identity.Domain.Core;
using Identity.Domain.Interfaces.Repositories;
using Identity.Domain.Interfaces.Services;
using Identity.Domain.Models;

namespace Identity.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly NotificationContext _notification;
        private readonly IUserRepository _repository;

        public UserService(NotificationContext notification,
            IUserRepository repository)
        {
            _notification = notification;
            _repository = repository;
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

        public async Task InsertAsync(User user)
        {
            await _repository.InsertAsync(user);
        }

        public async Task UpdateAsync(User user)
        {
            await _repository.UpdateAsync(user);
        }

        public async Task DeleteAsync(User user)
        {
            await _repository.DeleteAsync(user);
        }
    }
}
