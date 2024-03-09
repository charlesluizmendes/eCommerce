using Payment.Domain.Interfaces.Client;
using Payment.Domain.Interfaces.EventBus;
using Payment.Domain.Interfaces.Repositories;
using Payment.Domain.Interfaces.Services;
using Payment.Domain.Models;

namespace Payment.Domain.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repository;
        private readonly ITransactionService _transactionService;
        private readonly IBasketClient _client;
        private readonly IOrderEventBus _eventBus;

        public PaymentService(IPaymentRepository repository,
            ITransactionService transactionService,
            IBasketClient client,
            IOrderEventBus eventBus)
        {
            _repository = repository;
            _transactionService = transactionService;
            _client = client;
            _eventBus = eventBus;
        }

        public async Task<bool> InsertAsync(Models.Payment payment)
        {
            var basket = await _client.GetBasketByUserIdAsync(payment.UserId);

            if (basket == null)
                return false;

            var payment_ = await _repository.GetByBasketIdAsync(basket.Id);

            // Verifica se o Pagamento ja foi criado
            if (payment_ != null)
                return false;

            payment.BasketId = basket.Id;
            payment.Total = SumPriceItems(basket.Items);

            // Cria o Pagamento
            await _repository.InsertAsync(payment);

            // Cria a Transação
            await _transactionService.InsertAsync(new Transaction()
            {
                PaymentId = payment.Id
            });

            #region module Payment External

            /*
                Aqui pode-se utilizar Provedores de Pagamento Externo
                para validar a compra. Mas como trata-se de fins acadêmicos, 
                vou tratar toda compra como Aprovada (var confirm = true) e
                criar uma Order.
            */

            var confirm = true;

            #endregion

            // Obter a Transação 
            var transaction = await _transactionService.GetByPaymentIdAsync(payment.Id);

            if (!confirm)
            {
                // Atualiza a Transação para 'Cancelada'
                transaction.StatusId = 3;
                await _transactionService.UpdateAsync(transaction);

                return false;
            }
            
            // Atualiza a Transação para 'Aprovada'
            transaction.StatusId = 2;
            await _transactionService.UpdateAsync(transaction);

            // Remove o Carrinho
            await _client.DeleteBasketByUserIdAsync(payment.UserId);

            // Cria a Order
            await _eventBus.PublisherAsync(new Order()
            {
                PaymentId = payment.Id,
                Total = payment.Total,
                Basket = basket
            });

            return true;
        }

        #region private Methods

        private double SumPriceItems(List<Item> items)
        {
            double total = 0;

            foreach (var item in items)
            {
                total += (item.Price * item.Quantity);
            }

            return total;
        }

        #endregion
    }
}
