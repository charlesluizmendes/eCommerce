using Basket.Domain.Models;

namespace Basket.Domain.Interfaces.Repositories
{
    public interface IItemRepository
    {
        Task<Item> GetByIdAsync(int id);
        Task AddAsync(Item item);
        void Remove(Item item);
        Task SaveChangesAsync();
    }
}
