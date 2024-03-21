using System;
namespace Payment.Domain.Interfaces.Repositories
{
	public interface IUnitOfWork
	{
        ICardRepository CardRepository { get; }
        IPaymentRepository PaymentRepository { get; }
        IPixRepository PixRepository { get; }
        ITransactionRepository TransactionRepository { get; }

        void Commit();
    }
}

