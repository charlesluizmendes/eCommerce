using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Payment.Application.Handlers;
using Payment.Domain.Interfaces.Client;
using Payment.Domain.Interfaces.EventBus;
using Payment.Domain.Interfaces.Repositories;
using Payment.Domain.Interfaces.Services;
using Payment.Domain.Services;
using Payment.Infrastructure.Client;
using Payment.Infrastructure.Context;
using Payment.Infrastructure.EventBus;
using Payment.Infrastructure.Options;
using Payment.Infrastructure.Repositories;
using Payment.Application.Validators;
using Payment.Application.Interface;
using Payment.Application.Service;
using Polly;
using Polly.Extensions.Http;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// IoC

builder.Services.AddTransient<IBasketClient, BasketClient>();
builder.Services.AddTransient<IOrderEventBus, OrderEventBus>();
builder.Services.AddTransient<IPaymentRepository, PaymentRepository>();
builder.Services.AddTransient<ITransactionRepository, TransactionRepository>();
builder.Services.AddTransient<IPaymentService, PaymentService>();
builder.Services.AddTransient<ITransactionService, TransactionService>();
builder.Services.AddTransient<IPaymentAppService, PaymentAppService>();

builder.Services.AddTransient<BasketHttpClientHandler>();
builder.Services.AddHttpContextAccessor();

// Context

builder.Services.AddDbContext<PaymentContext>(option =>
     option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// FluentValidation

builder.Services.AddValidatorsFromAssemblyContaining<CreatePaymentCardViewModelValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreatePaymentPixViewModelValidator>();
builder.Services.AddFluentValidationAutoValidation();

// RabbitMQ

builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMq"));

// JWT

var accessToken = builder.Configuration.GetSection("AccessToken");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = accessToken["Iss"],
        ValidAudience = accessToken["Aud"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(accessToken["Secret"])),
        ClockSkew = TimeSpan.Zero,
        RequireExpirationTime = true
    };
});

// HttpClient

var basket = builder.Configuration.GetSection("Basket");

builder.Services.AddHttpClient("Basket", client =>
{
    client.BaseAddress = new Uri(basket["BaseUrl"]);
})
    .AddHttpMessageHandler<BasketHttpClientHandler>()
    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
    .AddPolicyHandler(GetRetryPolicy());

builder.Services.AddControllers();

// Swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Microservice Payment",
        Description = "Microservice of Payment",
        Version = "v1"
    });
    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = "Token Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme }
            },
            new[] { "readAccess", "writeAccess" }
        }
    });
});

// Polly

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                                                                    retryAttempt)));
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Swagger

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// JWT

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
