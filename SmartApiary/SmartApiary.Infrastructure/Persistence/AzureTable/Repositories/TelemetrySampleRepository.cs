using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartApiary.Application.Interfaces;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Models;
using SmartApiary.Infrastructure.Common.Options;
using SmartApiary.Infrastructure.Persistence.AzureSql.Repositories;
using SmartApiary.Infrastructure.Persistence.AzureTable.Common;
using SmartApiary.Infrastructure.Persistence.AzureTable.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Infrastructure.Persistence.AzureTable.Repositories
{
    internal class TelemetrySampleRepository : AzureTableRepository<TelemetrySample, TelemetrySampleEntity>, ITelemetrySampleRepository
    {

        public TelemetrySampleRepository(
            TableServiceClient tableServiceClient, ITableKeyProvider<TelemetrySample> keyProvider, IOptions<AzureTableOptions> options,
            ITableMapper<TelemetrySample, TelemetrySampleEntity> mapper) : 
            base( tableServiceClient.GetTableClient(options.Value.TelemetrySamplesTable), keyProvider, mapper)
        {

        }

        public async Task AddAsync(TelemetrySample reading, CancellationToken ct = default)
        {
            await base.UpsertAsync(reading, ct);
        }

        public async Task<IReadOnlyCollection<TelemetrySample>> GetByHiveAsync(string hiveId, DateTime from, DateTime to, CancellationToken ct = default)
        {
            string rowKeyFrom = (DateTime.MaxValue - to).Ticks.ToString("D20");
            string rowKeyTo = (DateTime.MaxValue - from).Ticks.ToString("D20");
            string filter = $"PartitionKey eq '{hiveId}' " +$"and RowKey ge '{rowKeyFrom}' " +$"and RowKey le '{rowKeyTo}'";
            List<TelemetrySample> results = await base.QueryAsync(filter, ct);
            
            return results.AsReadOnly();
        }

        public async Task<TelemetrySample?> GetLatestByHiveIdAsync(string hiveId, CancellationToken ct = default)
        {
            List<TelemetrySample> results = await base.QueryAsync($"PartitionKey eq '{hiveId}'", ct);

            return results.FirstOrDefault();
        }


    }
}
