using Payment.Domain.Models;

namespace Payment.Domain.Interfaces.EventBus
{
    public interface IOrderEventBus
    {
        Task PublisherAsync(Order order);
    }
}
