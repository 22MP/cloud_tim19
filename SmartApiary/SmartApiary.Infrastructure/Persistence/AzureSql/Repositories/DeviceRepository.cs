using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Models;
using SmartApiary.Domain.ValueObjects;
using SmartApiary.Infrastructure.Common.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Infrastructure.Persistence.AzureSql.Repositories
{
    internal class DeviceRepository : IDeviceRepository
    {
        private readonly ILogger _logger;
        private readonly string _connectionString;
        
        public DeviceRepository(IOptions<AzureSqlOptions> options, ILogger<DeviceRepository> logger)
        {
            _logger = logger;
            _connectionString = options.Value.ConnectionString;
        }

        public Task AddAsync(Device device, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(EntityId id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Device?> GetByAccessTokenAsync(string token, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Device?> GetByHiveIdAsync(EntityId hiveId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Device?> GetByIdAsync(EntityId id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Device?> GetBySerialNumberAsync(string sn, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Device device, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
