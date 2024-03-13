using Microsoft.Extensions.Logging;
using Order.Domain.Extensions;
using Order.Domain.Interfaces.Client;
using Order.Domain.Interfaces.Proxys;
using Order.Domain.Interfaces.Repositories;
using Order.Domain.Interfaces.Services;

namespace Order.Domain.Services
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly IOrderRepository _repository;
        private readonly IIdentityClient _client;
        private readonly IEmailProxy _emailProxy;

        public OrderService(ILogger<OrderService> logger,
            IOrderRepository repository,
            IIdentityClient client,
            IEmailProxy emailProxy)
        {
            _logger = logger;
            _repository = repository;
            _client = client;
            _emailProxy = emailProxy;
        }

        public async Task<bool> CreateOrderAsync(Models.Order order)
        {
            var existingOrder = await _repository.GetByBasketIdAsync(order.Basket.Id);

            // Verificar se a Order já foi criada
            if (existingOrder == null)
            {
                order.EmailSend = false;
                order.Create = DateTime.Now;

                await _repository.InsertAsync(order);
                await _repository.SaveChangesAsync();

                _logger.LogInformation($"Inserindo OrderId: {order.Id}");
            }     

            var user = await _client.GetUserByIdAsync(order.Basket.UserId);

            // Verifica se o Usuario existe
            if (user == null)
            {
                _logger.LogInformation($"Não foi possível encontrar o Usuario UserId: {order.Basket.UserId}");

                return false;
            }                

            // Cria o Email de Confirmação
            var text = EmailExtension.CreateEmailConfirmationAsync(order.Id, 
                order.Basket.Amount, 
                order.Basket.Items);

            _logger.LogInformation($"Criando Email OrderId: {order.Id}");

            // Envia o Email de Confirmação
            await _emailProxy.SendAsync("no-replay@infnet.com",
               user.Email,
               "Confirmacao de Pedido",
               text);

            order.EmailSend = true;

            _repository.UpdateAsync(order);
            await _repository.SaveChangesAsync();

            _logger.LogInformation($"Enviando Email OrderId: {order.Id}");

            return true;
        }       
    }
}
