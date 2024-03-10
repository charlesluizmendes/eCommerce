using Basket.Domain.Interfaces.Client;
using Basket.Domain.Interfaces.Repositories;
using Basket.Domain.Interfaces.Services;
using Basket.Domain.Models;

namespace Basket.Domain.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _repository;
        private readonly IBasketRepository _basketRepository;
        private readonly ICatalogClient _client;

        public ItemService(IItemRepository repository,
            IBasketRepository basketRepository,
            ICatalogClient client)
        {
            _repository = repository;
            _basketRepository = basketRepository;
            _client = client;
        }

        public async Task AddToBasketAsync(Item item)
        {
            var product = await _client.GetProductByIdAsync(item.ProductId);

            item.Name = product?.Name;
            item.Description = product?.Description;
            item.Price = product?.Price;

            var basket = await _basketRepository.GetByUserIdAsync(item.Basket.UserId);

            // Verificar se o usuário já possui um carrinho
            if (basket == null)
            {
                // Se o usuário não tiver um carrinho, crie um novo carrinho
                basket = new Models.Basket
                {
                    UserId = item.Basket.UserId,
                    Items = new List<Item>()
                };

                basket.Active = true;
                await _basketRepository.AddAsync(basket);
            }

            var item_ = basket.Items.FirstOrDefault(x => x.ProductId == item.ProductId && x.Active == true);

            // Verificar se o item já existe no carrinho
            if (item_ != null)
            {
                // Se o item já existir, apenas atualize a quantidade
                item_.Quantity += item.Quantity;
            }
            else
            {
                // Se o item não existir, crie um novo item e adicione ao carrinho
                item.Active = true;
                basket.Items.Add(item);
            }

            // Atualiza o valor total do Carrinho
            var activeItems = basket.Items.Where(x => x.Active == true).ToList();
            basket.Amount = activeItems.Sum(x => x?.Quantity * x?.Price);

            await _repository.SaveChangesAsync();
        }

        public async Task RemoveFromBasketAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);

            if (item != null)
            {
                // Remove a Quantidade do Item
                if (item.Quantity > 1)
                {
                    item.Quantity -= 1;
                }
                else
                {
                    // Remove o Item
                    item.Active = false;
                    _repository.Remove(item);
                }

                var basket = await _basketRepository.GetByUserIdAsync(item.Basket.UserId);

                // Atualiza o valor total do Carrinho
                var activeItems = basket.Items.Where(x => x.Active == true).ToList();
                basket.Amount = activeItems.Sum(x => x?.Quantity * x?.Price);

                // Verifique se o carrinho está vazio
                if (activeItems.Count == 0)
                {
                    // Remove o Carrinho
                    basket.Active = false; 
                    _basketRepository.Remove(basket);
                }

                await _repository.SaveChangesAsync();
            }
        }
    }
}
