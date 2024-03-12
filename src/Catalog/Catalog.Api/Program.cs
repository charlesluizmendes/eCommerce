using Catalog.Application.AutoMapper;
using Catalog.Application.Filters;
using Catalog.Domain.Core;
using Catalog.Domain.Interfaces.Identity;
using Catalog.Domain.Interfaces.Repositories;
using Catalog.Domain.Interfaces.Services;
using Catalog.Domain.Services;
using Catalog.Infrastructure.Context;
using Catalog.Infrastructure.Identity;
using Catalog.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(o =>

    // Filters
    o.Filters.Add<NotificationFilter>()
);

// IoC

builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<IUserIdentity, UserIdentity>();
builder.Services.AddScoped<NotificationContext>();

// Add HttpContext

builder.Services.AddHttpContextAccessor();

// AutoMapper

builder.Services.AddAutoMapper(typeof(MappingProfile));

// Context

builder.Services.AddDbContext<CatalogContext>(o =>
     o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

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

// Swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Microservice Catalog",
        Description = "Microservice of Catalog",
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
