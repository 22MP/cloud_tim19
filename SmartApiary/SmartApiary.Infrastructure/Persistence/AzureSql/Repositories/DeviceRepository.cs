using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums.Devices;
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

        public async Task AddAsync(Device device, CancellationToken ct = default)
        {
            const string sql = @"INSERT INTO Devices (Id, HiveId, SerialNumber, DeviceUuid, AccessToken, Status, RegisteredAt, PairedAt) 
            VALUES (@Id, @HiveId, @SerialNumber, @DeviceUuid, @AccessToken, @Status, @RegisteredAt, @PairedAt)";
            
            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync(ct);
                await using var cmd = new SqlCommand(sql, conn);
                MapParameters(cmd, device);
                await cmd.ExecuteNonQueryAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SQL] Error inserting device {Id}.", device.Id.Value);
                throw;
            }
        }

        public async Task DeleteAsync(EntityId id, CancellationToken ct = default)
        {
            const string sql = "DELETE FROM Devices WHERE Id = @Id";
            
            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync(ct);
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id.Value);
                await cmd.ExecuteNonQueryAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SQL] Error deleting device {Id}.", id.Value);
                throw;
            }
        }

        public async Task<Device?> GetByAccessTokenAsync(string token, CancellationToken ct = default)
        {
            const string sql = @"SELECT Id, HiveId, SerialNumber, DeviceUuid, AccessToken, Status, RegisteredAt, PairedAt 
            FROM Devices WHERE AccessToken = @Token";
            
            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync(ct);
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Token", token);
                await using var reader = await cmd.ExecuteReaderAsync(ct);
                if (!await reader.ReadAsync(ct)) return null;
                return MapToDevice(reader);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SQL] Error fetching device by Access Token.");
                throw;
            }
        }

        public async Task<Device?> GetByHiveIdAsync(EntityId hiveId, CancellationToken ct = default)
        {
            const string sql = @"SELECT Id, HiveId, SerialNumber, DeviceUuid, AccessToken, Status, RegisteredAt, PairedAt
            FROM Devices WHERE HiveId = @HiveId";
            
            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync(ct);
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@HiveId", hiveId.Value);
                await using var reader = await cmd.ExecuteReaderAsync(ct);
                if (!await reader.ReadAsync(ct)) return null;
                return MapToDevice(reader);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SQL] Error fetching device for hive {HiveId}.", hiveId.Value);
                throw;
            }
        }

        public async Task<Device?> GetByIdAsync(EntityId id, CancellationToken ct = default)
        {
            const string sql = @"SELECT Id, HiveId, SerialNumber, DeviceUuid, AccessToken, Status, RegisteredAt, PairedAt
            FROM Devices WHERE Id = @Id";
            
            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync(ct);
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id.Value);
                await using var reader = await cmd.ExecuteReaderAsync(ct);
                if (!await reader.ReadAsync(ct)) return null;
                return MapToDevice(reader);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SQL] Error fetching device {Id}.", id.Value);
                throw;
            }
        }

        public async Task<Device?> GetBySerialNumberAsync(string sn, CancellationToken ct = default)
        {
            const string sql = @"SELECT Id, HiveId, SerialNumber, DeviceUuid, AccessToken, Status, RegisteredAt, PairedAt
            FROM Devices WHERE SerialNumber = @SerialNumber";
            
            try
            {
                await using SqlConnection conn = new SqlConnection(_connectionString);
                await conn.OpenAsync(ct);
                await using SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@SerialNumber", sn);
                await using SqlDataReader reader = await cmd.ExecuteReaderAsync(ct);
                if (!await reader.ReadAsync(ct)) return null;
                return MapToDevice(reader);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SQL] Error fetching device by S/N (Serial Number): {SN}.", sn);
                throw;
            }
        }

        public async Task UpdateAsync(Device device, CancellationToken ct = default)
        {
            const string sql = @"UPDATE Devices SET DeviceUuid  = @DeviceUuid, AccessToken = @AccessToken, Status = @Status,
            PairedAt = @PairedAt WHERE Id = @Id";
            
            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync(ct);
                await using var cmd = new SqlCommand(sql, conn);
                MapParameters(cmd, device);
                await cmd.ExecuteNonQueryAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SQL] Error updating device {Id}.", device.Id.Value);
                throw;
            }
        }

        private static Device MapToDevice(SqlDataReader r)
        {
            Result<Device> result = Device.Load(
                id: r.GetString(r.GetOrdinal("Id")),
                hiveId: r.GetString(r.GetOrdinal("HiveId")),
                serialNumber: r.GetString(r.GetOrdinal("SerialNumber")),
                deviceUuid: r.IsDBNull(r.GetOrdinal("DeviceUuid")) ? null : r.GetString(r.GetOrdinal("DeviceUuid")),
                accessToken: r.IsDBNull(r.GetOrdinal("AccessToken")) ? null : r.GetString(r.GetOrdinal("AccessToken")),
                status: (DeviceStatus)r.GetInt32(r.GetOrdinal("Status")),
                registeredAt: r.GetDateTime(r.GetOrdinal("RegisteredAt")),
                pairedAt: r.IsDBNull(r.GetOrdinal("PairedAt")) ? null : r.GetDateTime(r.GetOrdinal("PairedAt"))
            );

            if(result.IsFailure)
            {
                throw new InvalidOperationException($"MapToDevice failed: {result.Error?.Message}");
            }
            return result.Value;
        }

        private static void MapParameters(SqlCommand cmd, Device d)
        {
            cmd.Parameters.AddWithValue("@Id", d.Id.Value);
            cmd.Parameters.AddWithValue("@HiveId", d.HiveId.Value);
            cmd.Parameters.AddWithValue("@SerialNumber", d.SerialNumber);
            cmd.Parameters.AddWithValue("@DeviceUuid", (object?)d.DeviceUuid ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AccessToken", (object?)d.AccessToken ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", (int)d.Status);
            cmd.Parameters.AddWithValue("@RegisteredAt", d.RegisteredAt);
            cmd.Parameters.AddWithValue("@PairedAt", (object?)d.PairedAt ?? DBNull.Value);
        }
    }
}
