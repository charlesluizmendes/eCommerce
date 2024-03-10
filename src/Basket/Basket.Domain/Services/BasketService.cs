using Basket.Domain.Interfaces.Repositories;
using Basket.Domain.Interfaces.Services;

namespace Basket.Domain.Services
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _repository;
        private readonly IItemRepository _itemRepository;

        public BasketService(IBasketRepository repository, 
            IItemRepository itemRepository)
        {
            _repository = repository;
            _itemRepository = itemRepository;
        }

        public Task<Models.Basket> GetByUserIdAsync(string userId)
        {
            return _repository.GetByUserIdAsync(userId);
        }

        public async Task RemoveAsync(int id)
        {
            var basket = await _repository.GetByIdAsync(id);

            if (basket != null) 
            {
                foreach (var item in basket.Items)
                {
                    // Remove todos os Items do Carrinho
                    item.Active = false;
                    _itemRepository.Update(item);
                }

                basket.Amount = 0;
                basket.Active = false;
                _repository.Update(basket);

                await _repository.SaveChangesAsync();
            }
        }
    }
}
