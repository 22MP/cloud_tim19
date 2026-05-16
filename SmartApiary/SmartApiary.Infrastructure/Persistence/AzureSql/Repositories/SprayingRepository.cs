using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Enums.Farmer;
using SmartApiary.Domain.Models;
using SmartApiary.Domain.ValueObjects;
using SmartApiary.Infrastructure.Common.Options;

namespace SmartApiary.Infrastructure.Persistence.AzureSql.Repositories
{
    internal class SprayingRepository(
        IOptions<AzureSqlOptions> options,
        ILogger<SprayingRepository> logger) : ISprayingRepository
    {
        private readonly string _connectionString = options.Value.ConnectionString;

        public async Task<SprayingSchedule?> GetByIdAsync(EntityId id, CancellationToken ct = default)
        {
            const string sql = @"
                SELECT Id, PlotId, ScheduledAt, DurationHours, Preparation,
                       Status, NotifiedBeekeeperCount, CreatedAt
                FROM SprayingSchedules
                WHERE Id = @Id";

            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync(ct);
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id.Value);

                await using var reader = await cmd.ExecuteReaderAsync(ct);
                if (!await reader.ReadAsync(ct)) return null;

                return MapToSchedule(reader);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[SQL] Error fetching spraying schedule {Id}.", id.Value);
                throw;
            }
        }

        public async Task<IReadOnlyCollection<SprayingSchedule>> GetByPlotIdAsync(EntityId plotId, CancellationToken ct = default)
        {
            const string sql = @"
                SELECT Id, PlotId, ScheduledAt, DurationHours, Preparation,
                       Status, NotifiedBeekeeperCount, CreatedAt
                FROM SprayingSchedules
                WHERE PlotId = @PlotId
                ORDER BY ScheduledAt DESC";

            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync(ct);
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@PlotId", plotId.Value);

                await using var reader = await cmd.ExecuteReaderAsync(ct);
                var schedules = new List<SprayingSchedule>();

                while (await reader.ReadAsync(ct))
                    schedules.Add(MapToSchedule(reader));

                return schedules.AsReadOnly();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[SQL] Error fetching spraying schedules for plot {PlotId}.", plotId.Value);
                throw;
            }
        }

        public async Task AddAsync(SprayingSchedule schedule, CancellationToken ct = default)
        {
            const string sql = @"
                INSERT INTO SprayingSchedules
                    (Id, PlotId, ScheduledAt, DurationHours, Preparation, Status, NotifiedBeekeeperCount, CreatedAt)
                VALUES
                    (@Id, @PlotId, @ScheduledAt, @DurationHours, @Preparation, @Status, @NotifiedBeekeeperCount, @CreatedAt)";

            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync(ct);
                await using var cmd = new SqlCommand(sql, conn);
                MapParameters(cmd, schedule);
                await cmd.ExecuteNonQueryAsync(ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[SQL] Error inserting spraying schedule {Id}.", schedule.Id.Value);
                throw;
            }
        }

        public async Task UpdateAsync(SprayingSchedule schedule, CancellationToken ct = default)
        {
            const string sql = @"
                UPDATE SprayingSchedules
                SET ScheduledAt            = @ScheduledAt,
                    DurationHours          = @DurationHours,
                    Preparation            = @Preparation,
                    Status                 = @Status,
                    NotifiedBeekeeperCount = @NotifiedBeekeeperCount
                WHERE Id = @Id";

            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync(ct);
                await using var cmd = new SqlCommand(sql, conn);
                MapParameters(cmd, schedule);
                await cmd.ExecuteNonQueryAsync(ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[SQL] Error updating spraying schedule {Id}.", schedule.Id.Value);
                throw;
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static SprayingSchedule MapToSchedule(SqlDataReader r)
        {
            return SprayingSchedule.Load(
                id: r.GetString(r.GetOrdinal("Id")),
                plotId: r.GetString(r.GetOrdinal("PlotId")),
                scheduledAt: r.GetDateTime(r.GetOrdinal("ScheduledAt")),
                durationHours: r.GetInt32(r.GetOrdinal("DurationHours")),
                preparation: r.IsDBNull(r.GetOrdinal("Preparation")) ? null : r.GetString(r.GetOrdinal("Preparation")),
                status: (SprayingStatus)r.GetInt32(r.GetOrdinal("Status")),
                notifiedBeekeeperCount: r.GetInt32(r.GetOrdinal("NotifiedBeekeeperCount")),
                createdAt: r.GetDateTime(r.GetOrdinal("CreatedAt"))
            ).Value;
        }

        private static void MapParameters(SqlCommand cmd, SprayingSchedule s)
        {
            cmd.Parameters.AddWithValue("@Id", s.Id.Value);
            cmd.Parameters.AddWithValue("@PlotId", s.PlotId.Value);
            cmd.Parameters.AddWithValue("@ScheduledAt", s.ScheduledAt);
            cmd.Parameters.AddWithValue("@DurationHours", s.DurationHours);
            cmd.Parameters.AddWithValue("@Preparation", (object?)s.Preparation ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", (int)s.Status);
            cmd.Parameters.AddWithValue("@NotifiedBeekeeperCount", s.NotifiedBeekeeperCount);
            cmd.Parameters.AddWithValue("@CreatedAt", s.CreatedAt);
        }
    }
}
