using SmartApiary.Domain.Models;
using SmartApiary.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Interfaces.Repositories
{
    public interface IHiveInspectionRepository
    {
        Task AddAsync(HiveInspection inspection, CancellationToken ct);
        Task UpdateAsync(HiveInspection inspection, CancellationToken ct);
        Task DeleteAsync(EntityId id, CancellationToken ct);
        Task<HiveInspection?> GetByIdAsync(EntityId id, CancellationToken ct);
        Task<IReadOnlyCollection<HiveInspection>> GetByHiveAsync(EntityId hiveId, CancellationToken ct);
    }
}
