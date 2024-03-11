using Basket.Domain.Models;

namespace Basket.Domain.Interfaces.Services
{
    public interface IItemService
    {
        Task<bool> AddToBasketAsync(Item item);
        Task<bool> RemoveFromBasketAsync(int id);
    }
}
