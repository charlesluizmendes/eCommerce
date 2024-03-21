using Payment.Domain.Interfaces.Repositories;
using Payment.Infrastructure.Context;

namespace Payment.Infrastructure.Repositories
{
	public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly PaymentContext _context;

        private ICardRepository _cardRepository;
        private IPaymentRepository _paymentRepository;
        private IPixRepository _pixRepository;
        private ITransactionRepository _transactionRepository;

        public UnitOfWork(PaymentContext context)
		{
            _context = context;
        }

        public ICardRepository CardRepository
        {
            get
            {
                if (_cardRepository == null)
                {
                    _cardRepository = new CardRepository(_context);
                }
                return _cardRepository;
            }
        }

        public IPaymentRepository PaymentRepository
        {
            get
            {
                if (_paymentRepository == null)
                {
                    _paymentRepository = new PaymentRepository(_context);
                }
                return _paymentRepository;
            }
        }

        public IPixRepository PixRepository
        {
            get
            {
                if (_pixRepository == null)
                {
                    _pixRepository = new PixRepository(_context);
                }
                return _pixRepository;
            }
        }

        public ITransactionRepository TransactionRepository
        {
            get
            {
                if (_transactionRepository == null)
                {
                    _transactionRepository = new TransactionRepository(_context);
                }
                return _transactionRepository;
            }
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

