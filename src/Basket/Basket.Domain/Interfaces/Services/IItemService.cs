using Basket.Domain.Models;

namespace Basket.Domain.Interfaces.Services
{
    public interface IItemService
    {
        Task AddToBasketAsync(Item item);
        Task RemoveFromBasketAsync(int id);
    }
}
