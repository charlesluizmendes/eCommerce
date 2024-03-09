using Microsoft.EntityFrameworkCore;
using Order.Domain.Interfaces.EventBus;
using Order.Domain.Interfaces.Repositories;
using Order.Domain.Interfaces.Services;
using Order.Domain.Services;
using Order.Infrastructure.Context;
using Order.Infrastructure.EventBus;
using Order.Infrastructure.Options;
using Order.Infrastructure.Repositories;
using Order.Worker;
using Polly.Extensions.Http;
using Polly;
using Order.Domain.Interfaces.Client;
using Order.Infrastructure.Client;
using Order.Domain.Interfaces.Proxys;
using Order.Infrastructure.Proxys;
using Microsoft.Extensions.Configuration;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        configurationBuilder
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", true, true)
            .AddEnvironmentVariables();

        var configurationRoot = configurationBuilder.Build();

    })
    .ConfigureServices((builder, services) =>
    {
        // IoC

        services.AddSingleton<IOrderService, OrderService>();
        services.AddSingleton<IOrderRepository, OrderRepository>();
        services.AddSingleton<IOrderEventBus, OrderEventBus>();
        services.AddSingleton<IIdentityClient, IdentityClient>();
        services.AddSingleton<IEmailProxy, EmailProxy>();

        // Context

        services.AddDbContext<OrderContext>(option =>
             option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), 
             ServiceLifetime.Singleton
        );

        // RabbitMQ

        services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMq"));

        // Email

        services.Configure<EmailConfiguration>(builder.Configuration.GetSection("Email"));

        // HttpClient

        var identity = builder.Configuration.GetSection("Identity");

        services.AddHttpClient("Identity", client =>
        {
            client.BaseAddress = new Uri(identity["BaseUrl"]);
        })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler(GetRetryPolicy());

        // Polly

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                                                                            retryAttempt)));
        }

        // Worker

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
