namespace Order.Domain.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<Models.Order> GetByBasketIdAsync(int id);
        Task InsertAsync(Models.Order order);
        Task SaveChangesAsync();
    }
}
