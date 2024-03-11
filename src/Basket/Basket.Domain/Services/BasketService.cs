using Basket.Domain.Interfaces.Identity;
using Basket.Domain.Interfaces.Repositories;
using Basket.Domain.Interfaces.Services;

namespace Basket.Domain.Services
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _repository;
        private readonly IItemRepository _itemRepository;
        private readonly IUserIdentity _identity;

        public BasketService(IBasketRepository repository, 
            IItemRepository itemRepository,
            IUserIdentity identity)
        {
            _repository = repository;
            _itemRepository = itemRepository;
            _identity = identity;
        }

        public Task<Models.Basket> GetAsync()
        {
            var userId = _identity.GetUserIdFromToken();

            return _repository.GetByUserIdAsync(userId);
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var basket = await _repository.GetByIdAsync(id);

            if (basket == null)
                return false;

            foreach (var item in basket.Items)
            {
                // Remove todos os Items do Carrinho
                item.Active = false;
                _itemRepository.Update(item);
            }

            await _itemRepository.SaveChangesAsync();

            basket.Amount = 0;
            basket.Active = false;
            _repository.Update(basket);

            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
