using SmartApiary.Domain.Models;
using SmartApiary.Domain.ValueObjects;

namespace SmartApiary.Application.Interfaces.Repositories
{
    public interface IApiaryRepository
    {
        Task<Apiary?> GetByIdAsync(EntityId id, CancellationToken ct = default);
        Task<Apiary?> GetByIdWithHivesAsync(EntityId id, CancellationToken ct = default);
        Task<IReadOnlyCollection<Apiary>> GetAllByOwnerAsync(string ownerId, CancellationToken ct = default);
        Task AddAsync(Apiary apiary, CancellationToken ct = default);
        Task UpdateAsync(Apiary apiary, CancellationToken ct = default);
        Task DeleteAsync(EntityId id, CancellationToken ct = default);
    }
}
