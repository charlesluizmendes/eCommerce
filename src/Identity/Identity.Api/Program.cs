using FluentValidation;
using FluentValidation.AspNetCore;
using Identity.Application.AutoMapper;
using Identity.Application.Filters;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddValidatorsFromAssemblyContaining<AddUserViewModelValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AlterUserViewModelValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateAccessTokenViewModelValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddControllers(options =>
    // Filters
    options.Filters.Add<NotificationFilter>()
);

// Swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Microservice Identity",
        Description = "Microservice of Identity",
        Version = "v1"
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
