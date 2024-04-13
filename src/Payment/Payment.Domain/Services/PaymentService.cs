using Payment.Domain.Core;
using Payment.Domain.Interfaces.Client;
using Payment.Domain.Interfaces.EventBus;
using Payment.Domain.Interfaces.Repositories;
using Payment.Domain.Interfaces.Services;
using Payment.Domain.Models;

namespace Payment.Domain.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly NotificationContext _notification;
        private readonly IUnitOfWork _uow;
        private readonly IBasketClient _client;
        private readonly IOrderEventBus _eventBus;

        public PaymentService(NotificationContext notification,
            IUnitOfWork uow,
            IBasketClient client,
            IOrderEventBus eventBus)
        {
            _notification = notification;
            _uow = uow;
            _client = client;
            _eventBus = eventBus;
        }

        public async Task CreatePaymentAsync(Models.Payment payment)
        {
            var basket = await _client.GetBaskeAsync();

            if (basket == null) 
            {
                _notification.AddNotification("Não foi encontrado nenhum Carrinho");

                return;
            }

            var payment_ = await _uow.PaymentRepository.GetByBasketIdAsync(basket.Id);

            // Verifica se o Pagamento ja foi criado
            if (payment_ != null)
            {
                _notification.AddNotification("O Pagamento já foi realizado");

                return;
            }

            payment.Amount = basket.Amount;
            payment.Create = DateTime.Now;
            payment.UserId = basket.UserId;
            payment.BasketId = basket.Id;            

            // Cria o Pagamento
            await _uow.PaymentRepository.InsertAsync(payment);

            if (payment.Card != null)
            {
                payment.Card.Number = HideCardNumber(payment.Card.Number);
                await _uow.CardRepository.InsertAsync(payment.Card);
            }

            if (payment.Pix != null)
            {
                await _uow.PixRepository.InsertAsync(payment.Pix);
            }

            _uow.Commit();

            // Cria a Transação
            await _uow.TransactionRepository.InsertAsync(new Transaction()
            {
                Create = DateTime.Now,
                PaymentId = payment.Id,
                StatusId = 1
            });
            _uow.Commit();

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
            var transaction = await _uow.TransactionRepository.GetByPaymentIdAsync(payment.Id);

            if (!confirm)
            {
                // Atualiza a Transação para 'Cancelada'
                transaction.Update = DateTime.Now;
                transaction.StatusId = 3;

                _uow.TransactionRepository.Update(transaction);
                _uow.Commit();

                _notification.AddNotification("A Compra foi cancelada");

                return;
            }

            // Atualiza a Transação para 'Aprovada'
            transaction.Update = DateTime.Now;
            transaction.StatusId = 2;

            _uow.TransactionRepository.Update(transaction);
            _uow.Commit();

            // Remove o Carrinho
            await _client.RemoveBasketByIdAsync(basket.Id);

            // Cria a Order
            await _eventBus.PublisherAsync(new Order()
            {
                PaymentId = payment.Id,
                Basket = basket
            });
        }

        #region Private Methods

        private static string HideCardNumber(string number)
        {
            // Mantém os primeiros e últimos 4 dígitos e substitui os restantes por asteriscos
            return number.Substring(0, 4) + new string('*', number.Length - 8) + number.Substring(number.Length - 4, 4);
        }

        #endregion
    }
}
