using Basket.Application.AutoMapper;
using Basket.Domain.Interfaces.Repositories;
using Basket.Domain.Interfaces.Services;
using Basket.Domain.Services;
using Basket.Infrastructure.Context;
using Basket.Infrastructure.Repositories;
using Basket.Application.Validators;
using Basket.Domain.Interfaces.Identity;
using Basket.Infrastructure.Identity;
using Basket.Domain.Core;
using Basket.Domain.Interfaces.Client;
using Basket.Application.Handlers;
using Basket.Infrastructure.Client;
using Basket.Application.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.AspNetCore;
using FluentValidation;
using Polly.Extensions.Http;
using Polly;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// IoC

builder.Services.AddTransient<IBasketService, BasketService>();
builder.Services.AddTransient<IBasketRepository, BasketRepository>();
builder.Services.AddTransient<IItemService, ItemService>();
builder.Services.AddTransient<IItemRepository, ItemRepository>();
builder.Services.AddTransient<ICatalogClient, CatalogClient>();
builder.Services.AddTransient<IUserIdentity, UserIdentity>();
builder.Services.AddScoped<NotificationContext>();
builder.Services.AddTransient<CatalogHttpClientHandler>();

// Add HttpContext

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

builder.Services.AddControllers(options =>
    // Filters
    options.Filters.Add<NotificationFilter>()
); ;

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
