using Application;
using Application.Services;
using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Abstractions.Validation;
using Domain.Entities;
using Domain.Exceptions;
using FluentValidation;
using Infrastructure;
using Infrastructure.Cache;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Application.Validators.Auth;
using Application.Validators.OrderLines;
using Application.Validators.Orders;
using Application.Validators.ProductCategories;
using Application.Validators.Products;
using Application.Validators.Shops;
using Application.Validators.Users;
using DokWokApi.Middlewares;
using Domain.DTOs.Models.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DokWokApi.Extensions;

public static class ServiceCollectionExtensions
{
    private const string CorsConfigErrorMessage = "Unable to get data from configuration";
    private const string AudienceNotFound = "Unable to get audience from configuration";
    private const string IssuerNotFound = "Unable to get issuer from configuration";
    private const string SubjectNotFound = "Unable to get subject from configuration";
    private const string KeyNotFound = "Unable to get key from configuration";
    private const string LifetimeNotFound = "Unable to get lifetime from configuration";

    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services, string policyName, ConfigurationManager configuration)
    {
        var reactHttpProjectUrl = configuration["AllowedCorsUrls:ReactHttpProject"] ??
                                  throw new ConfigurationException(CorsConfigErrorMessage);
        var reactHttpsProjectUrl = configuration["AllowedCorsUrls:ReactHttpsProject"] ??
                                   throw new ConfigurationException(CorsConfigErrorMessage);
        services.AddCors(opts =>
        {
            opts.AddPolicy(policyName, policy =>
            {
                policy.WithOrigins(reactHttpProjectUrl, reactHttpsProjectUrl)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("FoodStoreConnection");
        services.AddDbContext<StoreDbContext>(opts =>
        {
            opts.UseNpgsql(connectionString);
            opts.UseSnakeCaseNamingConvention();
        });

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
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        ValidatorOptions.Global.LanguageManager.Enabled = false;
        services.AddValidatorsFromAssembly(ApplicationAssemblyReference.Assembly);
        services.AddScoped<IUserServiceValidator, UserServiceValidator>();
        services.AddScoped<IAuthServiceValidator, AuthServiceValidator>();
        services.AddScoped<IOrderServiceValidator, OrderServiceValidator>();
        services.AddScoped<IOrderLineServiceValidator, OrderLineServiceValidator>();
        services.AddScoped<IProductCategoryServiceValidator, ProductCategoryServiceValidator>();
        services.AddScoped<IProductServiceValidator, ProductServiceValidator>();
        services.AddScoped<IShopServiceValidator, ShopServiceValidator>();

        return services;
    }
    
    public static IServiceCollection AddJwtConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<JwtConfiguration>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var audience = configuration["DOKWOK_JWT_AUDIENCE"]
                           ?? throw new ConfigurationException(AudienceNotFound);
            var issuer = configuration["DOKWOK_JWT_ISSUER"]
                         ?? throw new ConfigurationException(IssuerNotFound);
            var subject = configuration["DOKWOK_JWT_SUBJECT"]
                          ?? throw new ConfigurationException(SubjectNotFound);
            var key = configuration["DOKWOK_JWT_SECRET_KEY"]
                      ?? throw new ConfigurationException(KeyNotFound);
            var lifetime = configuration["DOKWOK_JWT_TOKENLIFETIME"]
                           ?? throw new ConfigurationException(LifetimeNotFound);
            JwtConfiguration jwtConfig = new()
            {
                Audience = audience,
                Issuer = issuer,
                Subject = subject,
                Key = key,
                Lifetime = lifetime,
            };
            return jwtConfig;
        });

        return services;
    }

    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ICacheService, CacheService>();
        services.AddScoped<IProductCategoryService, ProductCategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IOrderLineService, OrderLineService>();
        services.AddScoped<IShopService, ShopService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ISecurityTokenService<User, JwtSecurityToken>, JwtService>();

        return services;
    }

    public static IServiceCollection AddCacheDecorators(this IServiceCollection services)
    {
        services.Decorate<IProductCategoryService, CacheProductCategoryService>();
        services.Decorate<IProductService, CacheProductService>();
        services.Decorate<IOrderService, CacheOrderService>();
        services.Decorate<IOrderLineService, CacheOrderLineService>();
        services.Decorate<IShopService, CacheShopService>();
        services.Decorate<IUserService, CacheUserService>();
        services.Decorate<IAuthService, CacheAuthService>();
        
        return services;
    }

    public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services, ConfigurationManager configuration)
    {
        var audience = configuration["DOKWOK_JWT_AUDIENCE"]
                       ?? throw new ConfigurationException(AudienceNotFound);
        var issuer = configuration["DOKWOK_JWT_ISSUER"]
                     ?? throw new ConfigurationException(IssuerNotFound);
        var key = configuration["DOKWOK_JWT_SECRET_KEY"]
                  ?? throw new ConfigurationException(KeyNotFound);
        var encodedKey = Encoding.UTF8.GetBytes(key);
        SymmetricSecurityKey symmetricSecurityKey = new(encodedKey);
        TokenValidationParameters tokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = symmetricSecurityKey,
            ClockSkew = TimeSpan.Zero,
        };

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
    
    public static IServiceCollection AddMiddlewareServices(this IServiceCollection services)
    {
        services.AddTransient<GlobalExceptionHandlerMiddleware>();
        return services;
    }
}
