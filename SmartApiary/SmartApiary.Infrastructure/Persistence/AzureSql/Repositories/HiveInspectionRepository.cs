using Microsoft.Data.SqlClient;
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
    internal class HiveInspectionRepository : IHiveInspectionRepository
    {
        private readonly string _connectionString;

        public HiveInspectionRepository(IOptions<AzureSqlOptions> options)
        {
            _connectionString = options.Value.ConnectionString;
        }

        public async Task AddAsync(HiveInspection i, CancellationToken ct)
        {
            const string sql = @"
            INSERT INTO HiveInspections
            (Id, HiveId, InspectionDate, BottomBoardColor,
             HoneyFrames, HoneyKg, BroodFrames, QueenPresent, Notes, CreatedAt)
            VALUES
            (@Id, @HiveId, @InspectionDate, @BottomBoardColor,
             @HoneyFrames, @HoneyKg, @BroodFrames, @QueenPresent, @Notes, @CreatedAt)";

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync(ct);
            await using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@Id", i.Id.Value);
            cmd.Parameters.AddWithValue("@HiveId", i.HiveId.Value);
            cmd.Parameters.AddWithValue("@InspectionDate", i.InspectionDate);
            cmd.Parameters.AddWithValue("@BottomBoardColor", i.BottomBoardColor);
            cmd.Parameters.AddWithValue("@HoneyFrames", i.HoneyFrames);
            cmd.Parameters.AddWithValue("@HoneyKg", i.HoneyKg);
            cmd.Parameters.AddWithValue("@BroodFrames", i.BroodFrames);
            cmd.Parameters.AddWithValue("@QueenPresent", i.QueenPresent);
            cmd.Parameters.AddWithValue("@Notes", (object?)i.Notes ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedAt", i.CreatedAt);

            await cmd.ExecuteNonQueryAsync(ct);
        }

        public async Task<IReadOnlyCollection<HiveInspection>> GetByHiveAsync(EntityId hiveId, CancellationToken ct)
        {
            const string sql = @"
        SELECT * FROM HiveInspections
        WHERE HiveId = @HiveId
        ORDER BY InspectionDate DESC";

            var list = new List<HiveInspection>();

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync(ct);
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@HiveId", hiveId.Value);

            await using var reader = await cmd.ExecuteReaderAsync(ct);

            while (await reader.ReadAsync(ct))
            {
                list.Add(HiveInspection.Create(
                    reader["HiveId"].ToString()!,
                    (DateTime)reader["InspectionDate"],
                    reader["BottomBoardColor"].ToString()!,
                    (int)reader["HoneyFrames"],
                    Convert.ToDouble(reader["HoneyKg"]),
                    (int)reader["BroodFrames"],
                    (bool)reader["QueenPresent"],
                    reader["Notes"] as string,
                    (DateTime)reader["CreatedAt"]
                ).Value);
            }

            return list;
        }

        public async Task DeleteAsync(EntityId id, CancellationToken ct)
        {
            const string sql = "DELETE FROM HiveInspections WHERE Id=@Id";

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync(ct);
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id.Value);

            await cmd.ExecuteNonQueryAsync(ct);
        }

        public Task<HiveInspection?> GetByIdAsync(EntityId id, CancellationToken ct)
            => throw new NotImplementedException();

        public Task UpdateAsync(HiveInspection inspection, CancellationToken ct)
            => throw new NotImplementedException();
    }
}
