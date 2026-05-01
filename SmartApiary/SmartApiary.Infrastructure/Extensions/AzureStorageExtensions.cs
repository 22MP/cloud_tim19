using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Azure.Data.Tables;
using Microsoft.Extensions.DependencyInjection;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Storage;
using SmartApiary.Infrastructure.Persistence.AzureBlob.Storages;
using SmartApiary.Infrastructure.Persistence.AzureSql.Repositories;
using SmartApiary.Infrastructure.Persistence.AzureTable.Repositories;
using SmartApiary.Infrastructure.Persistence.AzureTable.Common;
using SmartApiary.Infrastructure.Persistence.AzureTable.Mappers;
using SmartApiary.Infrastructure.Persistence.AzureTable.KeyProviders;
using SmartApiary.Infrastructure.Persistence.AzureTable.Entities;
using SmartApiary.Domain.Models;

namespace SmartApiary.Infrastructure.Extensions
{
    internal static class AzureStorageExtensions
    {
        public static IServiceCollection AddAzureSql(
            this IServiceCollection services)
        {
            services.AddScoped<IApiaryRepository, ApiaryRepository>();
            services.AddScoped<IHiveRepository, HiveRepository>();
            return services;
        }

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
            services.AddSingleton(_ => new BlobServiceClient(connectionString));
            services.AddScoped<IApiaryImageStorage, ApiaryImageStorage>();
            return services;
        }

        public static IServiceCollection AddAzureQueues(
            this IServiceCollection services,
            string connectionString)
        {
            
            services.AddSingleton(_ => new QueueServiceClient(connectionString, new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            }));
            return services;
        }
    }
}