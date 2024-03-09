using Basket.Domain.Models;

namespace Basket.Domain.Interfaces.Repositories
{
    public interface IBasketRepository
    {
        Task<Models.Basket> GetByIdAsync(int id);
        Task<Models.Basket> GetByUserIdAsync(string userId);
        Task AddAsync(Models.Basket basket);
        void Remove(Models.Basket basket);
        Task SaveChangesAsync();
    }
}
