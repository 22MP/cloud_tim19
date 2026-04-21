using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Extensions.DependencyInjection;
using SmartApiary.Application.Interfaces.Storage;

namespace SmartApiary.Infrastructure.Extensions;

internal static class AzureStorageExtensions
{
    public static IServiceCollection AddAzureTables(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddSingleton(new TableServiceClient(connectionString));
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