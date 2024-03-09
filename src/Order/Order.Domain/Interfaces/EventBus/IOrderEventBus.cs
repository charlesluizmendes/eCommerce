namespace Order.Domain.Interfaces.EventBus
{
    public interface IOrderEventBus
    {
        Task SubscriberAsync(Func<Models.Order, CancellationToken, Task<bool>> filme);
    }
}
