using FluentValidation;
using FluentValidation.AspNetCore;
using Identity.Application.AutoMapper;
using Identity.Application.Validators;
using Identity.Domain.Interfaces.Repositories;
using Identity.Domain.Interfaces.Services;
using Identity.Domain.Models;
using Identity.Domain.Services;
using Identity.Infraestructure.Context;
using Identity.Infraestructure.Options;
using Identity.Infraestructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// IoC

builder.Services.AddTransient<IAccessTokenService, AccessTokenService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IAccessTokenRepository, AccessTokenRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();

// AutoMapper

builder.Services.AddAutoMapper(typeof(MappingProfile));

// Option

builder.Services.Configure<AccessTokenConfiguration>(builder.Configuration.GetSection("AccessToken"));

// Context

builder.Services.AddDbContext<IdentityContext>(option =>
     option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Identity

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<IdentityContext>();

// FluentValidation

builder.Services.AddValidatorsFromAssemblyContaining<CreateUserViewModelValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateUserViewModelValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateAccessTokenViewModelValidator>();
builder.Services.AddFluentValidationAutoValidation();

// Swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Microservice Identity",
        Description = "Microservice of Identity",
        Version = "v1"
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Swagger

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
