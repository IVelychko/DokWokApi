using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DokWokApi.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string ConnectionStringName = "TestFoodStoreConnection";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var configuration = config.Build();
            var connectionString = configuration.GetConnectionString(ConnectionStringName);

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<StoreDbContext>>();
                // Register the DbContext with the PostgreSQL connection string
                services.AddDbContext<StoreDbContext>(options =>
                {
                    options.UseNpgsql(connectionString);
                });
            });
        });

        builder.UseEnvironment("Development");
    }
}
