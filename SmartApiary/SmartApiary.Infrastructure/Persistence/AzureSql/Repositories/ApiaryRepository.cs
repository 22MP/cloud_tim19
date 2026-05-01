using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Models;
using SmartApiary.Domain.ValueObjects;
using SmartApiary.Infrastructure.Common.Options;
using SmartApiary.Infrastructure.Persistence.AzureSql.Entities;
using System.Data;

namespace SmartApiary.Infrastructure.Persistence.AzureSql.Repositories
{
    internal class ApiaryRepository(
        IOptions<AzureSqlOptions> options,
        ILogger<ApiaryRepository> logger) : IApiaryRepository
    {
        private readonly string _connectionString = options.Value.ConnectionString;

        public async Task<Apiary?> GetByIdAsync(EntityId id, CancellationToken ct = default)
        {
            const string sql = @"
                SELECT Id, OwnerId, Name, Latitude, Longitude, Description, ImageUrl, ThumbnailUrl, CreatedAt
                FROM Apiaries
                WHERE Id = @Id";

            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync(ct);
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id.Value);

                await using var reader = await cmd.ExecuteReaderAsync(ct);
                if (!await reader.ReadAsync(ct)) return null;

                return MapToApiary(reader);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[SQL] Error fetching apiary {Id}.", id.Value);
                throw;
            }
        }

        public async Task<Apiary?> GetByIdWithHivesAsync(EntityId id, CancellationToken ct = default)
        {
            const string sql = @"
                SELECT a.Id, a.OwnerId, a.Name, a.Latitude, a.Longitude, a.Description, a.ImageUrl, a.ThumbnailUrl, a.CreatedAt,
                       h.Id AS HiveId, h.Label, h.Type, h.BoxColor, h.QueenAge, h.Note, h.CreatedAt AS HiveCreatedAt
                FROM Apiaries a
                LEFT JOIN Hives h ON h.ApiaryId = a.Id
                WHERE a.Id = @Id";

            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync(ct);
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id.Value);

                await using var reader = await cmd.ExecuteReaderAsync(ct);

                Apiary? apiary = null;
                var hives = new List<Hive>();

                while (await reader.ReadAsync(ct))
                {
                    if (apiary is null)
                        apiary = MapToApiary(reader);

                    if (!reader.IsDBNull(reader.GetOrdinal("HiveId")))
                        hives.Add(MapToHive(reader));
                }

                if (apiary is null) return null;

                return Apiary.Load(
                    apiary.Id.Value, apiary.OwnerId, apiary.Name,
                    apiary.Latitude, apiary.Longitude, apiary.Description,
                    apiary.ImageUrl, apiary.ThumbnailUrl, apiary.CreatedAt,
                    hives).Value;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[SQL] Error fetching apiary {Id} with hives.", id.Value);
                throw;
            }
        }

        public async Task<IReadOnlyCollection<Apiary>> GetAllByOwnerAsync(string ownerId, CancellationToken ct = default)
        {
            const string sql = @"
                SELECT a.Id, a.OwnerId, a.Name, a.Latitude, a.Longitude, a.Description, a.ImageUrl, a.ThumbnailUrl, a.CreatedAt,
                       COUNT(h.Id) AS HiveCount
                FROM Apiaries a
                LEFT JOIN Hives h ON h.ApiaryId = a.Id
                WHERE a.OwnerId = @OwnerId
                GROUP BY a.Id, a.OwnerId, a.Name, a.Latitude, a.Longitude, a.Description, a.ImageUrl, a.ThumbnailUrl, a.CreatedAt";

            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync(ct);
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@OwnerId", ownerId);

                await using var reader = await cmd.ExecuteReaderAsync(ct);
                var apiaries = new List<Apiary>();

                while (await reader.ReadAsync(ct))
                    apiaries.Add(MapToApiary(reader));

                return apiaries.AsReadOnly();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[SQL] Error fetching apiaries for owner {OwnerId}.", ownerId);
                throw;
            }
        }

        public async Task AddAsync(Apiary apiary, CancellationToken ct = default)
        {
            const string sql = @"
                INSERT INTO Apiaries (Id, OwnerId, Name, Latitude, Longitude, Description, ImageUrl, ThumbnailUrl, CreatedAt, Location)
                VALUES (@Id, @OwnerId, @Name, @Latitude, @Longitude, @Description, @ImageUrl, @ThumbnailUrl, @CreatedAt,
                        geography::Point(@Latitude, @Longitude, 4326))";

            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync(ct);
                await using var cmd = new SqlCommand(sql, conn);
                MapParameters(cmd, apiary);
                await cmd.ExecuteNonQueryAsync(ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[SQL] Error inserting apiary {Id}.", apiary.Id.Value);
                throw;
            }
        }

        public async Task UpdateAsync(Apiary apiary, CancellationToken ct = default)
        {
            const string sql = @"
                UPDATE Apiaries
                SET Name = @Name, Latitude = @Latitude, Longitude = @Longitude,
                    Description = @Description, ImageUrl = @ImageUrl, ThumbnailUrl = @ThumbnailUrl,
                    Location = geography::Point(@Latitude, @Longitude, 4326)
                WHERE Id = @Id";

            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync(ct);
                await using var cmd = new SqlCommand(sql, conn);
                MapParameters(cmd, apiary);
                await cmd.ExecuteNonQueryAsync(ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[SQL] Error updating apiary {Id}.", apiary.Id.Value);
                throw;
            }
        }

        public async Task DeleteAsync(EntityId id, CancellationToken ct = default)
        {
            const string sql = "DELETE FROM Apiaries WHERE Id = @Id";

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
                logger.LogError(ex, "[SQL] Error deleting apiary {Id}.", id.Value);
                throw;
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static Apiary MapToApiary(SqlDataReader r)
        {
            return Apiary.Load(
                id:           r.GetString(r.GetOrdinal("Id")),
                ownerId:      r.GetString(r.GetOrdinal("OwnerId")),
                name:         r.GetString(r.GetOrdinal("Name")),
                latitude:     r.GetDouble(r.GetOrdinal("Latitude")),
                longitude:    r.GetDouble(r.GetOrdinal("Longitude")),
                description:  r.IsDBNull(r.GetOrdinal("Description")) ? null : r.GetString(r.GetOrdinal("Description")),
                imageUrl:     r.IsDBNull(r.GetOrdinal("ImageUrl"))    ? null : r.GetString(r.GetOrdinal("ImageUrl")),
                thumbnailUrl: r.IsDBNull(r.GetOrdinal("ThumbnailUrl"))? null : r.GetString(r.GetOrdinal("ThumbnailUrl")),
                createdAt:    r.GetDateTime(r.GetOrdinal("CreatedAt"))
            ).Value;
        }

        private static Hive MapToHive(SqlDataReader r)
        {
            return Hive.Load(
                id:        r.GetString(r.GetOrdinal("HiveId")),
                apiaryId:  r.GetString(r.GetOrdinal("Id")),
                label:     r.GetString(r.GetOrdinal("Label")),
                type:      (Domain.Enums.Apiaries.HiveType)r.GetInt32(r.GetOrdinal("Type")),
                boxColor:  r.IsDBNull(r.GetOrdinal("BoxColor"))  ? null : r.GetString(r.GetOrdinal("BoxColor")),
                queenAge:  r.IsDBNull(r.GetOrdinal("QueenAge"))  ? null : r.GetInt32(r.GetOrdinal("QueenAge")),
                note:      r.IsDBNull(r.GetOrdinal("Note"))      ? null : r.GetString(r.GetOrdinal("Note")),
                createdAt: r.GetDateTime(r.GetOrdinal("HiveCreatedAt"))
            ).Value;
        }

        private static void MapParameters(SqlCommand cmd, Apiary apiary)
        {
            cmd.Parameters.AddWithValue("@Id",           apiary.Id.Value);
            cmd.Parameters.AddWithValue("@OwnerId",      apiary.OwnerId);
            cmd.Parameters.AddWithValue("@Name",         apiary.Name);
            cmd.Parameters.AddWithValue("@Latitude",     apiary.Latitude);
            cmd.Parameters.AddWithValue("@Longitude",    apiary.Longitude);
            cmd.Parameters.AddWithValue("@Description",  (object?)apiary.Description  ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ImageUrl",     (object?)apiary.ImageUrl     ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ThumbnailUrl", (object?)apiary.ThumbnailUrl ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedAt",    apiary.CreatedAt);
        }
    }
}
