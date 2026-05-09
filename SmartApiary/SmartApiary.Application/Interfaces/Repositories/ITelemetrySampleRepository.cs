using SmartApiary.Domain.Models;
using SmartApiary.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Interfaces.Repositories
{
    public interface ITelemetrySampleRepository
    {
        Task AddAsync(TelemetrySample reading, CancellationToken ct = default);
        Task<IReadOnlyCollection<TelemetrySample>> GetByHiveAsync(string hiveId, DateTime from, DateTime to, CancellationToken ct = default);
        Task<TelemetrySample?> GetLatestByHiveIdAsync(string hiveId, CancellationToken ct = default);
    }
}
