using Identity.Domain.Interfaces.Repositories;
using Identity.Domain.Interfaces.Services;
using Identity.Domain.Models;

namespace Identity.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }       

        public async Task<User> GetByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
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

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _repository.GetUserByEmailAsync(email);
        }
    }
}
