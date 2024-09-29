using Application.Behaviors;
using DokWokApi.Extensions;
using DokWokApi.Services;
using Domain.Helpers;
using Domain.Services;
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
    opts.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

builder.Services.AddDbContext<StoreDbContext>(opts =>
{
    var connectionString = builder.Configuration.GetConnectionString("FoodStoreConnection");
    opts.UseNpgsql(connectionString);
    opts.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
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
var passwordHasher = app.Services.GetRequiredService<PasswordHasher>();
await SeedData.SeedDatabaseAsync(context, passwordHasher);

await app.RunAsync();
