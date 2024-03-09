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

                await _basketRepository.AddAsync(basket);
            }

            var item_ = basket.Items.FirstOrDefault(x => x.ProductId == item.ProductId);

            // Verificar se o item já existe no carrinho
            if (item_ != null)
            {
                // Se o item já existir, apenas atualize a quantidade
                item_.Quantity += item.Quantity;
            }
            else
            {
                // Se o item não existir, crie um novo item e adicione ao carrinho
                basket.Items.Add(item);
            }

            await _repository.SaveChangesAsync();
        }

        public async Task RemoveFromBasketAsync(int id)
        {
            // Verificar se o usuário já possui um carrinho
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
                    _repository.Remove(item);
                }

                // Verifique se o carrinho está vazio
                if (item.Basket.Items.Count == 0)
                {
                    // Remove o Carrinho
                    _basketRepository.Remove(item.Basket);
                }

                // Salve as alterações no banco de dados
                await _repository.SaveChangesAsync();
            }
        }
    }
}
