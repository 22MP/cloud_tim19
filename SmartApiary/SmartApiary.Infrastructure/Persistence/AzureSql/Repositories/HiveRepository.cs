using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Enums.Apiaries;
using SmartApiary.Domain.Models;
using SmartApiary.Domain.ValueObjects;
using SmartApiary.Infrastructure.Common.Options;

namespace SmartApiary.Infrastructure.Persistence.AzureSql.Repositories
{
    internal class HiveRepository(
        IOptions<AzureSqlOptions> options,
        ILogger<HiveRepository> logger) : IHiveRepository
    {
        private readonly string _connectionString = options.Value.ConnectionString;

        public async Task<Hive?> GetByIdAsync(EntityId id, CancellationToken ct = default)
        {
            const string sql = @"
                SELECT Id, ApiaryId, Label, Type, BoxColor, QueenAge, Note, CreatedAt
                FROM Hives WHERE Id = @Id";

            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync(ct);
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id.Value);

                await using var reader = await cmd.ExecuteReaderAsync(ct);
                if (!await reader.ReadAsync(ct)) return null;

                return MapToHive(reader);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[SQL] Error fetching hive {Id}.", id.Value);
                throw;
            }
        }

        public async Task<IReadOnlyCollection<Hive>> GetAllByApiaryAsync(EntityId apiaryId, CancellationToken ct = default)
        {
            const string sql = @"
                SELECT Id, ApiaryId, Label, Type, BoxColor, QueenAge, Note, CreatedAt
                FROM Hives WHERE ApiaryId = @ApiaryId
                ORDER BY Label";

            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync(ct);
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ApiaryId", apiaryId.Value);

                await using var reader = await cmd.ExecuteReaderAsync(ct);
                var hives = new List<Hive>();

                while (await reader.ReadAsync(ct))
                    hives.Add(MapToHive(reader));

                return hives.AsReadOnly();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[SQL] Error fetching hives for apiary {ApiaryId}.", apiaryId.Value);
                throw;
            }
        }

        public async Task AddAsync(Hive hive, CancellationToken ct = default)
        {
            const string sql = @"
                INSERT INTO Hives (Id, ApiaryId, Label, Type, BoxColor, QueenAge, Note, CreatedAt)
                VALUES (@Id, @ApiaryId, @Label, @Type, @BoxColor, @QueenAge, @Note, @CreatedAt)";

            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync(ct);
                await using var cmd = new SqlCommand(sql, conn);
                MapParameters(cmd, hive);
                await cmd.ExecuteNonQueryAsync(ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[SQL] Error inserting hive {Id}.", hive.Id.Value);
                throw;
            }
        }

        public async Task UpdateAsync(Hive hive, CancellationToken ct = default)
        {
            const string sql = @"
                UPDATE Hives
                SET Label = @Label, Type = @Type, BoxColor = @BoxColor, QueenAge = @QueenAge, Note = @Note
                WHERE Id = @Id";

            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync(ct);
                await using var cmd = new SqlCommand(sql, conn);
                MapParameters(cmd, hive);
                await cmd.ExecuteNonQueryAsync(ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[SQL] Error updating hive {Id}.", hive.Id.Value);
                throw;
            }
        }

        public async Task DeleteAsync(EntityId id, CancellationToken ct = default)
        {
            const string sql = "DELETE FROM Hives WHERE Id = @Id";

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
                logger.LogError(ex, "[SQL] Error deleting hive {Id}.", id.Value);
                throw;
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static Hive MapToHive(SqlDataReader r)
        {
            return Hive.Load(
                id:        r.GetString(r.GetOrdinal("Id")),
                apiaryId:  r.GetString(r.GetOrdinal("ApiaryId")),
                label:     r.GetString(r.GetOrdinal("Label")),
                type:      (HiveType)r.GetInt32(r.GetOrdinal("Type")),
                boxColor:  r.IsDBNull(r.GetOrdinal("BoxColor")) ? null : r.GetString(r.GetOrdinal("BoxColor")),
                queenAge:  r.IsDBNull(r.GetOrdinal("QueenAge")) ? null : r.GetInt32(r.GetOrdinal("QueenAge")),
                note:      r.IsDBNull(r.GetOrdinal("Note"))     ? null : r.GetString(r.GetOrdinal("Note")),
                createdAt: r.GetDateTime(r.GetOrdinal("CreatedAt"))
            ).Value;
        }

        private static void MapParameters(SqlCommand cmd, Hive hive)
        {
            cmd.Parameters.AddWithValue("@Id",        hive.Id.Value);
            cmd.Parameters.AddWithValue("@ApiaryId",  hive.ApiaryId.Value);
            cmd.Parameters.AddWithValue("@Label",     hive.Label);
            cmd.Parameters.AddWithValue("@Type",      (int)hive.Type);
            cmd.Parameters.AddWithValue("@BoxColor",  (object?)hive.BoxColor ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@QueenAge",  (object?)hive.QueenAge ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Note",      (object?)hive.Note     ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedAt", hive.CreatedAt);
        }
    }
}
