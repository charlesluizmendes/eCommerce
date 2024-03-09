namespace Order.Domain.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task InsertAsync(Models.Order order);
    }
}
