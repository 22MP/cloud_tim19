using SmartApiary.Domain.Models;
using SmartApiary.Infrastructure.Persistence.AzureTable.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Infrastructure.Persistence.AzureTable.KeyProviders
{
    internal class TelemetrySampleTableKeyProvider : ITableKeyProvider<TelemetrySample>
    {
        public string GetPartitionKey(TelemetrySample model)
        {
            return model.HiveId.Value;
        }

        public string GetRowKey(TelemetrySample model)
        {
            return (DateTime.MaxValue - model.RecordedAt).Ticks.ToString("D20");
        }
    }
}
