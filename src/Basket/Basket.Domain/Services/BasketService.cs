using Basket.Domain.Core;
using Basket.Domain.Interfaces.Identity;
using Basket.Domain.Interfaces.Repositories;
using Basket.Domain.Interfaces.Services;

namespace Basket.Domain.Services
{
    public class BasketService : IBasketService
    {
        private readonly NotificationContext _notification;
        private readonly IBasketRepository _repository;
        private readonly IItemRepository _itemRepository;
        private readonly IUserIdentity _identity;

        public BasketService(NotificationContext notification,
            IBasketRepository repository, 
            IItemRepository itemRepository,
            IUserIdentity identity)
        {
            _notification = notification;
            _repository = repository;
            _itemRepository = itemRepository;
            _identity = identity;
        }

        public Task<Models.Basket> GetAsync()
        {
            var userId = _identity.GetUserIdFromToken();
            var basket = _repository.GetByUserIdAsync(userId);

            if (basket == null) 
                _notification.AddNotification("Não foi encontrado nenhum Carrinho");
            
            return basket;
        }

        public async Task RemoveAsync(int id)
        {
            var basket = await _repository.GetByIdAsync(id);

            if (basket == null)
            {
                _notification.AddNotification("Não foi encontrado nenhum Carrinho");

                return;
            }

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
        }
    }
}
