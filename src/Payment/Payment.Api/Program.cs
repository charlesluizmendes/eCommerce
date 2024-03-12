using FluentValidation.AspNetCore;
using FluentValidation;
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
using Payment.Application.Filters;
using Payment.Infrastructure.Identity;
using Payment.Application.AutoMapper;
using Payment.Domain.Interfaces.Identity;
using Payment.Domain.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Polly;
using Polly.Extensions.Http;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(o =>

    // Filters
    o.Filters.Add<NotificationFilter>()
);

// IoC

builder.Services.AddTransient<IBasketClient, BasketClient>();
builder.Services.AddTransient<IOrderEventBus, OrderEventBus>();
builder.Services.AddTransient<IPaymentRepository, PaymentRepository>();
builder.Services.AddTransient<ICardRepository, CardRepository>();
builder.Services.AddTransient<IPixRepository, PixRepository>();
builder.Services.AddTransient<ITransactionRepository, TransactionRepository>();
builder.Services.AddTransient<IPaymentService, PaymentService>();
builder.Services.AddTransient<IUserIdentity, UserIdentity>();
builder.Services.AddTransient<BasketHttpClientHandler>();
builder.Services.AddScoped<NotificationContext>();

// Add HttpContext

builder.Services.AddHttpContextAccessor();

// AutoMapper

builder.Services.AddAutoMapper(typeof(MappingProfile));

// Context

builder.Services.AddDbContext<PaymentContext>(o =>
     o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// FluentValidation

builder.Services.AddValidatorsFromAssemblyContaining<CreatePaymentCardViewModelValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreatePaymentPixViewModelValidator>();
builder.Services.AddFluentValidationAutoValidation();

// RabbitMQ

builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMq"));

// JWT

var accessToken = builder.Configuration.GetSection("AccessToken");

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessToken["Secret"])),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization(o =>
{
    o.DefaultPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();
});

// HttpClient

var basket = builder.Configuration.GetSection("Basket");

builder.Services.AddHttpClient("Basket", o =>
{
    o.BaseAddress = new Uri(basket["BaseUrl"]);
})
    .AddHttpMessageHandler<BasketHttpClientHandler>()
    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
    .AddPolicyHandler(GetRetryPolicy());

// Swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Microservice Payment",
        Description = "Microservice of Payment",
        Version = "v1"
    });
    o.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = "Token Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
    });
    o.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// ModelState Validation

builder.Services.Configure<ApiBehaviorOptions>(o =>
{
    o.SuppressMapClientErrors = true;
    o.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .SelectMany(state => state.Value.Errors)
            .Select(error => error.ErrorMessage);

        return new BadRequestObjectResult(new Notification()
        {
            Errors = new List<string>(errors)
        });
    };
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
