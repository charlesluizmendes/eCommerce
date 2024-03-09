using Microsoft.Extensions.Options;
using Order.Domain.Interfaces.EventBus;
using Order.Infrastructure.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Order.Infrastructure.EventBus
{
    public class OrderEventBus : IOrderEventBus
    {
        private readonly IOptions<RabbitMqConfiguration> _rabbitMqConfiguration;
        private IConnection _connection;
        private IModel _model;

        private const int Retry = 60000;
        private const string Name = nameof(Order);
        private const string Queue = ".Queue";
        private const string Exchange = ".Exchange";
        private const string DeadLetterQueue = ".DeadLetter.Queue";
        private const string DeadLetterExchange = ".DeadLetter.Exchange";

        public OrderEventBus(IOptions<RabbitMqConfiguration> rabbitMqConfiguration)
        {
            _rabbitMqConfiguration = rabbitMqConfiguration;

            CreateConnect();
            CreateModel();
            CreateQueue();
        }

        public async Task SubscriberAsync(Func<Domain.Models.Order, CancellationToken, Task<bool>> order)
        {
            var consumer = new AsyncEventingBasicConsumer(_model);
            consumer.Received += async (ch, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var text = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<Domain.Models.Order>(text);

                    if (message != null)
                    {
                        var result = order(message, default).GetAwaiter().GetResult();

                        if (result)
                        {
                            _model.BasicAck(ea.DeliveryTag, false);
                        }
                        else
                        {
                            _model.BasicNack(ea.DeliveryTag, false, false);
                        }
                    }
                    else
                    {
                        _model.BasicAck(ea.DeliveryTag, false);
                    }

                    await Task.Yield();
                }
                catch (Exception)
                {
                    _model.BasicNack(ea.DeliveryTag, false, false);
                }
            };

            _model.BasicConsume(Name + Queue, false, consumer);

            await Task.Yield();
        }

        #region private Methods

        private void CreateConnect()
        {
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqConfiguration.Value.HostName,
                Port = _rabbitMqConfiguration.Value.Port,
                UserName = _rabbitMqConfiguration.Value.UserName,
                Password = _rabbitMqConfiguration.Value.Password,
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
        }

        private void CreateModel()
        {
            _model = _connection.CreateModel();
        }

        private void CreateQueue()
        {
            var argsQueue = new Dictionary<string, object>()
            {
                { "x-dead-letter-exchange", Name + DeadLetterExchange },
                { "x-dead-letter-routing-key", Name + DeadLetterQueue }
            };

            _model.ExchangeDeclare(exchange: Name + Exchange, type: ExchangeType.Fanout);
            _model.QueueDeclare(queue: Name + Queue, durable: true, exclusive: false, autoDelete: false, arguments: argsQueue);
            _model.QueueBind(queue: Name + Queue, exchange: Name + Exchange, routingKey: string.Empty, arguments: null);

            _model.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var argsDeadLetter = new Dictionary<string, object>()
            {
               { "x-dead-letter-exchange", Name + Exchange },
               { "x-message-ttl", Retry }
            };

            _model.ExchangeDeclare(exchange: Name + DeadLetterExchange, type: ExchangeType.Fanout);
            _model.QueueDeclare(queue: Name + DeadLetterQueue, durable: true, exclusive: false, autoDelete: false, arguments: argsDeadLetter);
            _model.QueueBind(queue: Name + DeadLetterQueue, exchange: Name + DeadLetterExchange, routingKey: string.Empty, arguments: null);
        }

        #endregion
    }
}
