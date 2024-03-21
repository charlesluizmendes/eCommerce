using Basket.Domain.Interfaces.Repositories;
using Basket.Infrastructure.Context;

namespace Basket.Infrastructure.Repositories
{
	public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly BasketContext _context;

        private IBasketRepository _basketRepository;
        private IItemRepository _itemRepository;

		public UnitOfWork(BasketContext context)
		{
            _context = context;
        }

        public IBasketRepository BasketRepository
        {
            get
            {
                if (_basketRepository == null)
                {
                    _basketRepository = new BasketRepository(_context);
                }
                return _basketRepository;
            }
        }

        public IItemRepository ItemRepository
        {
            get
            {
                if (_itemRepository == null)
                {
                    _itemRepository = new ItemRepository(_context);
                }
                return _itemRepository;
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

