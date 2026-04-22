using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartApiary.Infrastructure.Common.Options;
using SmartApiary.Infrastructure.Extensions;

namespace SmartApiary.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind settings from appsettings.json
        services.Configure<AzureTableOptions>(configuration.GetSection("AzureTableOptions"));

        // Get connection string
        var tableConn = configuration.GetValue<string>("AzureTableOptions:ConnectionString")
            ?? throw new InvalidOperationException("AzureTableOptions:ConnectionString is not configured.");

        
        services
            .AddServices()      
            .AddAzureTables(tableConn);

        return services;
    }
}