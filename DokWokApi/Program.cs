using DokWokApi.Constants;
using DokWokApi.Extensions;
using DokWokApi.Helpers;
using DokWokApi.Middlewares;
using Domain.Shared;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
const string policyName = CorsPolicyNames.ReactProject;

builder.Services.AddCorsConfiguration(policyName, builder.Configuration);

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddControllers();

builder.Services.AddStackExchangeRedisCache(opts =>
{
    var connection = builder.Configuration.GetConnectionString("RedisConnection");
    opts.Configuration = connection;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext(builder.Configuration);
builder.Services.AddCustomRepositories();
builder.Services.AddValidators();
builder.Services.AddJwtConfiguration();
builder.Services.AddSingleton<TokenValidationParametersAccessor>();
builder.Services.AddCustomServices();
builder.Services.AddCacheDecorators();

builder.Services.AddSwaggerSetup();

builder.Services.AddMiddlewareServices();

builder.Services.AddJwtBearerAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors(policyName);

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApp");
    });
}

app.MapControllers();

await DatabaseHelper.SeedDatabaseAsync(app);

await app.RunAsync();

#pragma warning disable S1118 // Utility classes should not have public constructors
public partial class Program { }
#pragma warning restore S1118 // Utility classes should not have public constructors