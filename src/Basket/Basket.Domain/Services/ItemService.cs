using Basket.Domain.Core;
using Basket.Domain.Interfaces.Client;
using Basket.Domain.Interfaces.Identity;
using Basket.Domain.Interfaces.Repositories;
using Basket.Domain.Interfaces.Services;
using Basket.Domain.Models;

namespace Basket.Domain.Services
{
    public class ItemService : IItemService
    {
        private readonly NotificationContext _notification;
        private readonly IItemRepository _repository;
        private readonly IBasketRepository _basketRepository;
        private readonly IUserIdentity _identity;
        private readonly ICatalogClient _client;

        public ItemService(IItemRepository repository,
            NotificationContext notification,
            IBasketRepository basketRepository,
            IUserIdentity identity,
            ICatalogClient client)
        {
            _notification = notification;
            _repository = repository;
            _basketRepository = basketRepository;
            _identity = identity;
            _client = client;
        }

        public async Task AddToBasketAsync(Item item)
        {
            var product = await _client.GetProductByIdAsync(item.ProductId);

            if (product == null) 
            {
                _notification.AddNotification("Não foi encontrado nenhum Produto");

                return;
            }

            item.Name = product.Name;
            item.Description = product.Description;
            item.Price = product.Price;

            var userId = _identity.GetUserIdFromToken();

            if (string.IsNullOrEmpty(userId))
            {
                _notification.AddNotification("Token Inválido");

                return;
            }
                
            var basket = await _basketRepository.GetByUserIdAsync(userId);

            // Verificar se o usuário já possui um carrinho
            if (basket == null)
            {
                // Se o usuário não tiver um carrinho, crie um novo carrinho
                basket = new Models.Basket
                {
                    UserId = userId,
                    Items = new List<Item>(),
                    Create = DateTime.Now,
                    Active = true
                };

                await _basketRepository.AddAsync(basket);
            }

            var existingItem = basket.Items.FirstOrDefault(x => x.ProductId == item.ProductId && x.Active);

            // Verificar se o item já existe no carrinho
            if (existingItem != null)
            {
                // Se o item já existir, apenas atualize a quantidade
                existingItem.Update = DateTime.Now;
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                // Se o item não existir, crie um novo item e adicione ao carrinho
                item.Create = DateTime.Now;
                item.Active = true;
                basket.Items.Add(item);
            }

            // Atualiza o valor total do Carrinho
            var activeItems = basket.Items.Where(x => x.Active).ToList();
            basket.Amount = activeItems.Sum(x => x.Quantity * x.Price);

            // Salva as alterações
            await _repository.SaveChangesAsync();
        }

        public async Task RemoveFromBasketAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);

            // Verifica se o Item existe
            if (item == null) 
            {
                _notification.AddNotification("Não foi encontrado nenhum Item");

                return; 
            }

            // Remove a Quantidade do Item
            if (item.Quantity > 1)
            {
                item.Update = DateTime.Now;
                item.Quantity -= 1;
            }
            else
            {
                // Remove o Item
                item.Delete = DateTime.Now;
                item.Active = false;
                _repository.Update(item);
            }

            var userId = _identity.GetUserIdFromToken();

            if (string.IsNullOrEmpty(userId))
            {
                _notification.AddNotification("Token Inválido");

                return;
            }

            var basket = await _basketRepository.GetByUserIdAsync(userId);

            // Atualiza o valor total do Carrinho
            var activeItems = basket.Items.Where(x => x.Active).ToList();
            basket.Amount = activeItems.Sum(x => x.Quantity * x.Price);

            // Verifique se o carrinho está vazio
            if (activeItems.Count == 0)
            {
                // Remove o Carrinho
                basket.Delete = DateTime.Now;
                basket.Active = false;
                _basketRepository.Update(basket);
            }

            // Salva as alterações
            await _repository.SaveChangesAsync();
        }
    }
}
