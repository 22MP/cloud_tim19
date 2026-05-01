using SmartApiary.Domain.Models;
using SmartApiary.Domain.ValueObjects;

namespace SmartApiary.Application.Interfaces.Repositories
{
    public interface IHiveRepository
    {
        Task<Hive?> GetByIdAsync(EntityId id, CancellationToken ct = default);
        Task<IReadOnlyCollection<Hive>> GetAllByApiaryAsync(EntityId apiaryId, CancellationToken ct = default);
        Task AddAsync(Hive hive, CancellationToken ct = default);
        Task UpdateAsync(Hive hive, CancellationToken ct = default);
        Task DeleteAsync(EntityId id, CancellationToken ct = default);
    }
}
