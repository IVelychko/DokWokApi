using Carter;
using DokWokApi.Extensions;
using DokWokApi.Services;
using Domain.Entities;
using Domain.Helpers;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
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
    opts.RegisterServicesFromAssembly(Application.AssemblyReference.Assembly);
});

builder.Services.AddDbContext<StoreDbContext>(opts =>
{
    var connectionString = builder.Configuration["ConnectionStrings:FoodStoreConnection"];
    opts.UseSqlServer(connectionString);
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

builder.Services.AddCarter();

var app = builder.Build();

app.UseExceptionHandler(x => { });

app.UseCors(policyName);

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapCarter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApp");
    });
}

var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<StoreDbContext>();
var roleManager = app.Services.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
var userManager = app.Services.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
await SeedData.SeedDatabaseAsync(context, roleManager, userManager);

await app.RunAsync();
