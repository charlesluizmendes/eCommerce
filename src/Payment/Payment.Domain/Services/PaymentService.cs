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
        private readonly ICardRepository _cardRepository;
        private readonly IPixRepository _pixRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IBasketClient _client;
        private readonly IOrderEventBus _eventBus;

        public PaymentService(IPaymentRepository repository,
            ICardRepository cardRepository,
            IPixRepository pixRepository,
            ITransactionRepository transactionRepository,
            IBasketClient client,
            IOrderEventBus eventBus)
        {
            _repository = repository;
            _cardRepository = cardRepository;
            _pixRepository = pixRepository;
            _transactionRepository = transactionRepository;
            _client = client;
            _eventBus = eventBus;
        }

        public async Task<bool> CreatePaymentAsync(Models.Payment payment)
        {
            var basket = await _client.GetBaskeAsync();

            if (basket == null)
                return false;

            var payment_ = await _repository.GetByBasketIdAsync(basket.Id);

            // Verifica se o Pagamento ja foi criado
            if (payment_ != null)
                return false;

            payment.UserId = basket.UserId;
            payment.BasketId = basket.Id;
            payment.Amount = basket.Amount;

            // Cria o Pagamento
            await _repository.InsertAsync(payment);

            if (payment.Card != null)
            {
                payment.Card.Number = HideCardNumber(payment.Card.Number);
                await _cardRepository.InsertAsync(payment.Card);
                await _cardRepository.SaveChangesAsync();   
            }

            if (payment.Pix != null)
            {
                await _pixRepository.InsertAsync(payment.Pix);
                await _pixRepository.SaveChangesAsync();    
            }

            await _repository.SaveChangesAsync();

            // Cria a Transação
            await _transactionRepository.InsertAsync(new Transaction()
            {
                PaymentId = payment.Id,
                StatusId = 1
            });
            await _transactionRepository.SaveChangesAsync();

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
            var transaction = await _transactionRepository.GetByPaymentIdAsync(payment.Id);

            if (!confirm)
            {
                // Atualiza a Transação para 'Cancelada'
                transaction.StatusId = 3;
                _transactionRepository.Update(transaction);
                await _transactionRepository.SaveChangesAsync();

                return false;
            }
            
            // Atualiza a Transação para 'Aprovada'
            transaction.StatusId = 2;
            _transactionRepository.Update(transaction);
            await _transactionRepository.SaveChangesAsync();

            // Remove o Carrinho
            await _client.RemoveBasketByIdAsync(basket.Id);

            // Cria a Order
            await _eventBus.PublisherAsync(new Order()
            {
                PaymentId = payment.Id,
                Basket = basket
            });

            return true;
        }

        #region Private Methods

        private static string HideCardNumber(string numeroCartao)
        {
            // Mantém os primeiros e últimos 4 dígitos e substitui os restantes por asteriscos
            string numeroOculto = numeroCartao.Substring(0, 4) +
                                  new string('*', numeroCartao.Length - 8) +
                                  numeroCartao.Substring(numeroCartao.Length - 4, 4);

            return numeroOculto;
        }

        #endregion
    }
}
