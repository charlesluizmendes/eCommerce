using Basket.Application.AutoMapper;
using Basket.Domain.Interfaces.Repositories;
using Basket.Domain.Interfaces.Services;
using Basket.Domain.Services;
using Basket.Infrastructure.Context;
using Basket.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Polly.Extensions.Http;
using Polly;
using System.Text;
using Basket.Application.Handlers;
using Microsoft.OpenApi.Models;
using Basket.Domain.Interfaces.Client;
using Basket.Infrastructure.Client;
using FluentValidation.AspNetCore;
using FluentValidation;
using Basket.Application.Validators;

var builder = WebApplication.CreateBuilder(args);

// IoC

builder.Services.AddTransient<IBasketService, BasketService>();
builder.Services.AddTransient<IBasketRepository, BasketRepository>();
builder.Services.AddTransient<IItemService, ItemService>();
builder.Services.AddTransient<IItemRepository, ItemRepository>();
builder.Services.AddTransient<ICatalogClient, CatalogClient>();

builder.Services.AddTransient<CatalogHttpClientHandler>();
builder.Services.AddHttpContextAccessor();

// AutoMapper

builder.Services.AddAutoMapper(typeof(MappingProfile));

// Context

builder.Services.AddDbContext<BasketContext>(option =>
     option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// FluentValidation

builder.Services.AddValidatorsFromAssemblyContaining<AddItemViewModelValidator>();
builder.Services.AddFluentValidationAutoValidation();

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

var catalog = builder.Configuration.GetSection("Catalog");

builder.Services.AddHttpClient("Catalog", client =>
{
    client.BaseAddress = new Uri(catalog["BaseUrl"]);
})
    .AddHttpMessageHandler<CatalogHttpClientHandler>()
    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
    .AddPolicyHandler(GetRetryPolicy());

builder.Services.AddControllers();

// Swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Microservice Basket",
        Description = "Microservice of Basket",
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
