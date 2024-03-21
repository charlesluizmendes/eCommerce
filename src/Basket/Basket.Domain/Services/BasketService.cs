using Basket.Domain.Core;
using Basket.Domain.Interfaces.Identity;
using Basket.Domain.Interfaces.Repositories;
using Basket.Domain.Interfaces.Services;

namespace Basket.Domain.Services
{
    public class BasketService : IBasketService
    {
        private readonly NotificationContext _notification;
        private readonly IUnitOfWork _uow;
        private readonly IUserIdentity _identity;

        public BasketService(NotificationContext notification,
            IUnitOfWork uow,
            IUserIdentity identity)
        {
            _notification = notification;
            _uow = uow;
            _identity = identity;
        }

        public async Task<Models.Basket> GetAsync()
        {
            var userId = _identity.GetUserIdFromToken();

            if (string.IsNullOrEmpty(userId))
            {
                _notification.AddNotification("Token Inválido");

                return null;
            }

            var basket = await _uow.BasketRepository.GetByUserIdAsync(userId);

            if (basket == null) 
                _notification.AddNotification("Não foi encontrado nenhum Carrinho");
            
            return basket;
        }

        public async Task RemoveAsync(int id)
        {
            var basket = await _uow.BasketRepository.GetByIdAsync(id);

            if (basket == null)
            {
                _notification.AddNotification("Não foi encontrado nenhum Carrinho");

                return;
            }

            foreach (var item in basket.Items)
            {
                // Remove todos os Items do Carrinho
                item.Delete = DateTime.Now;
                item.Active = false;

                _uow.ItemRepository.Update(item);
            }

            // Remover o Carrinho
            basket.Amount = 0;
            basket.Delete = DateTime.Now;
            basket.Active = false;

            _uow.BasketRepository.Update(basket);

            _uow.Commit();
        }
    }
}
