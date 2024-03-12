using FluentValidation.AspNetCore;
using FluentValidation;
using Identity.Application.AutoMapper;
using Identity.Application.Validators;
using Identity.Domain.Core;
using Identity.Domain.Interfaces.Identity;
using Identity.Domain.Interfaces.Repositories;
using Identity.Domain.Interfaces.Services;
using Identity.Domain.Models;
using Identity.Domain.Services;
using Identity.Infraestructure.Context;
using Identity.Infraestructure.Options;
using Identity.Infraestructure.Repositories;
using Identity.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Identity.Application.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(o =>

    // Filters
    o.Filters.Add<NotificationFilter>()
);

// IoC

builder.Services.AddTransient<IAccessTokenService, AccessTokenService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUserIdentity, UserIdentity>();
builder.Services.AddScoped<NotificationContext>();

// Add HttpContext

builder.Services.AddHttpContextAccessor();

// AutoMapper

builder.Services.AddAutoMapper(typeof(MappingProfile));

// Option

builder.Services.Configure<AccessTokenConfiguration>(builder.Configuration.GetSection("AccessToken"));

// Context

builder.Services.AddDbContext<IdentityContext>(o =>
     o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Identity

builder.Services.AddIdentity<User, IdentityRole>(o =>
{
    o.Password.RequireDigit = false;
    o.Password.RequiredLength = 3;
    o.Password.RequireNonAlphanumeric = false;
    o.Password.RequireUppercase = false;
    o.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<IdentityContext>();

// FluentValidation

builder.Services.AddValidatorsFromAssemblyContaining<AddUserViewModelValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AlterUserViewModelValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateAccessTokenViewModelValidator>();
builder.Services.AddFluentValidationAutoValidation();

// JWT

var accessToken = builder.Configuration.GetSection("AccessToken");

builder.Services.AddAuthentication()
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.SaveToken = true;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessToken["Secret"]))
        };
    });

// Swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Microservice Identity",
        Description = "Microservice of Identity",
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

app.UseAuthorization();

app.MapControllers();

app.Run();
