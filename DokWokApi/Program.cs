using Application.Behaviors;
using DokWokApi.Extensions;
using DokWokApi.Services;
using Domain.Abstractions.Services;
using Domain.Shared;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

const string policyName = "ReactProjectCorsPolicy";

builder.Services.AddCorsConfiguration(policyName, builder.Configuration);

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddMediatR(opts =>
{
    opts.RegisterServicesFromAssembly(Application.ApplicationAssemblyReference.Assembly);
    opts.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationWithResponseBehavior<,>));
    opts.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

builder.Services.AddStackExchangeRedisCache(opts =>
{
    var connection = builder.Configuration.GetConnectionString("RedisConnection");
    opts.Configuration = connection;
});

builder.Services.AddDbContext<StoreDbContext>(opts =>
{
    var connectionString = builder.Configuration.GetConnectionString("FoodStoreConnection");
    opts.UseNpgsql(connectionString);
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<TokenValidationParametersAccessor>();

builder.Services.AddValidators();
builder.Services.AddCustomRepositories();
builder.Services.AddCustomServices();

builder.Services.AddSwaggerSetup();

builder.Services.AddJwtBearerAuthentication(builder.Configuration);
builder.Services.AddAuthorizationServicesAndPolicies();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler(x => { });

app.UseCors(policyName);

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.AddAllEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApp");
    });
}

var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<StoreDbContext>();
var passwordHasher = app.Services.GetRequiredService<IPasswordHasher>();
await SeedData.SeedDatabaseAsync(context, passwordHasher);

await app.RunAsync();

#pragma warning disable S1118 // Utility classes should not have public constructors
public partial class Program { }
#pragma warning restore S1118 // Utility classes should not have public constructors