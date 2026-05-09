using SmartApiary.Domain.Models;
using SmartApiary.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Interfaces.Repositories
{
    public interface IDeviceRepository
    {
        Task<Device?> GetByIdAsync(EntityId id, CancellationToken ct = default);
        Task<Device?> GetBySerialNumberAsync(string sn, CancellationToken ct = default);
        Task<Device?> GetByHiveIdAsync(EntityId hiveId, CancellationToken ct = default);
        Task<Device?> GetByAccessTokenAsync(string token, CancellationToken ct = default);
        Task AddAsync(Device device, CancellationToken ct = default);
        Task UpdateAsync(Device device, CancellationToken ct = default);
        Task DeleteAsync(EntityId id, CancellationToken ct = default);
    }
}
