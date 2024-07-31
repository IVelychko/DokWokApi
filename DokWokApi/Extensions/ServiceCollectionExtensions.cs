﻿using DokWokApi.Helpers;
using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Abstractions.Validation;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Helpers;
using Domain.Models.User;
using Domain.Services;
using Domain.Validation;
using Infrastructure;
using Infrastructure.Repositories;
using Infrastructure.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;

namespace DokWokApi.Extensions;

public static class ServiceCollectionExtensions
{
    private const string ErrorMessage = "Unable to get data from configuration";

    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services, string policyName, ConfigurationManager configuration)
    {
        services.AddCors(opts =>
        {
            opts.AddPolicy(policyName, policy =>
            {
                policy.WithOrigins(configuration["AllowedCorsUrls:ReactHttpProject"] ?? throw new ConfigurationException(ErrorMessage),
                    configuration["AllowedCorsUrls:ReactHttpsProject"] ?? throw new ConfigurationException(ErrorMessage))
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<ProductCategory>, ProductCategoryRepositoryValidator>();
        services.AddScoped<IValidator<Product>, ProductRepositoryValidator>();
        services.AddScoped<IValidator<Order>, OrderRepositoryValidator>();
        services.AddScoped<IValidator<OrderLine>, OrderLineRepositoryValidator>();
        services.AddScoped<IValidator<Shop>, ShopRepositoryValidator>();
        services.AddScoped<IValidator<RefreshToken>, RefreshTokenRepositoryValidator>();
        services.AddScoped<IUserRepositoryValidator, UserRepositoryValidator>();
        services.AddScoped<IUserServiceValidator, UserServiceValidator>();

        return services;
    }

    public static IServiceCollection AddCustomRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderLineRepository, OrderLineRepository>();
        services.AddScoped<IShopRepository, ShopRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<IProductCategoryService, ProductCategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IOrderLineService, OrderLineService>();
        services.AddScoped<IShopService, ShopService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISecurityTokenService<UserModel, JwtSecurityToken>, JwtService>();

        return services;
    }

    public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>(opts =>
        {
            opts.Password.RequiredLength = 6;
            opts.Password.RequireNonAlphanumeric = false;
            opts.Password.RequireLowercase = false;
            opts.Password.RequireUppercase = true;
            opts.Password.RequireDigit = true;
            opts.User.RequireUniqueEmail = true;
        }).AddEntityFrameworkStores<StoreDbContext>();

        return services;
    }

    public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services, ConfigurationManager configuration)
    {
        var tokenValidationParametersAccessor = new TokenValidationParametersAccessor(configuration);
        var tokenValidationParameters = tokenValidationParametersAccessor.Regular;

        services.AddAuthentication(opts =>
        {
            opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(opts =>
        {
            opts.TokenValidationParameters = tokenValidationParameters;
        });

        return services;
    }

    public static IServiceCollection AddAuthorizationServicesAndPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicyNames.Admin, opts => opts.RequireRole(UserRoles.Admin))
            .AddPolicy(AuthorizationPolicyNames.Customer, opts => opts.RequireRole(UserRoles.Customer))
            .AddPolicy(AuthorizationPolicyNames.AdminAndCustomer, opts => opts.RequireRole(UserRoles.Admin, UserRoles.Customer));

        return services;
    }

    public static IServiceCollection AddSwaggerSetup(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opts =>
        {
            opts.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] " +
                "and then your token in the text input below",
            });
            opts.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
            opts.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "DokWokApi",
                Version = "v1"
            });
        });

        return services;
    }
}
