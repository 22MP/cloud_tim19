using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Extensions.DependencyInjection;
using SmartApiary.Application.Interfaces; 
using SmartApiary.Application.Interfaces.Storage;
using SmartApiary.Domain.Models;
using SmartApiary.Infrastructure.Persistence.AzureTable.Common;
using SmartApiary.Infrastructure.Persistence.AzureTable.Entities;
using SmartApiary.Infrastructure.Persistence.AzureTable.KeyProviders;
using SmartApiary.Infrastructure.Persistence.AzureTable.Mappers;
using SmartApiary.Infrastructure.Persistence.AzureTable.Repositories;
using SmartApiary.Application.Interfaces.Repositories;

namespace SmartApiary.Infrastructure.Extensions;

internal static class AzureStorageExtensions
{
    public static IServiceCollection AddAzureTables(
        this IServiceCollection services,
        string connectionString)
    {
        
        services.AddSingleton(new TableServiceClient(connectionString));

        
        services.AddSingleton<ITableMapper<User, UserEntity>, UserTableMapper>();

       
        services.AddSingleton<ITableKeyProvider<User>, UserTableKeyProvider>();

        
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    public static IServiceCollection AddAzureBlobs(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddSingleton(sp => new BlobServiceClient(connectionString));

        
        services.AddScoped<IFileStorage, Persistence.AzureBlob.AzureBlobStorage>();

        return services;
    }

    public static IServiceCollection AddAzureQueues(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddSingleton(sp => new QueueServiceClient(
            connectionString,
            new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            }));

        return services;
    }
}