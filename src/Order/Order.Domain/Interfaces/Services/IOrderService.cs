namespace Order.Domain.Interfaces.Services
{
    public interface IOrderService
    {
        Task<bool> CreateOrderAsync(Models.Order order);
    }
}
