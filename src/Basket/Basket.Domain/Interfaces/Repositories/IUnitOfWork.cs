namespace Basket.Domain.Interfaces.Repositories
{
	public interface IUnitOfWork
	{
        IBasketRepository BasketRepository { get; }
        IItemRepository ItemRepository { get; }

        void Commit();
    }
}

