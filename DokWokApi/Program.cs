using DokWokApi.BLL.Infrastructure;
using DokWokApi.DAL;
using DokWokApi.Extensions;
using DokWokApi.Infrastructure;
using DokWokApi.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddControllers();

const string policyName = "ReactProjectCorsPolicy";

builder.Services.AddCorsConfiguration(policyName, builder.Configuration);

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opts =>
{
    opts.IdleTimeout = TimeSpan.FromMinutes(30);
    opts.Cookie.Name = "DokWokApi.Session";
    opts.Cookie.IsEssential = true;
    opts.Cookie.HttpOnly = true;
});

builder.Services.AddDbContext<StoreDbContext>(opts =>
{
    var connectionString = builder.Configuration["ConnectionStrings:FoodStoreConnection"];
    opts.UseSqlServer(connectionString, opts =>
    {
        opts.MigrationsAssembly("DokWokApi");
    });
});

builder.Services.AddIdentityConfiguration();

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

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();
//app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApp");
    });
}

await SeedData.SeedDatabaseAsync(app);

await app.RunAsync();
