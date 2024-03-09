using Order.Domain.Interfaces.EventBus;
using Order.Domain.Interfaces.Services;

namespace Order.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IOrderEventBus _eventBus;
        private readonly IOrderService _orderService;

        public Worker(ILogger<Worker> logger,
            IOrderEventBus eventBus,
            IOrderService orderService)
        {
            _logger = logger;
            _eventBus = eventBus;
            _orderService = orderService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _eventBus.SubscriberAsync(async (order, stoppingToken) =>
                await _orderService.CreateOrderAsync(order)
            );
        }
    }
}
