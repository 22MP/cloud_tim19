using SmartApiary.Domain.Enums.Apiaries;

namespace SmartApiary.Infrastructure.Persistence.AzureSql.Entities
{
    internal class HiveEntity
    {
        public string Id { get; set; } = string.Empty;
        public string ApiaryId { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public HiveType Type { get; set; }
        public string? BoxColor { get; set; }
        public int? QueenAge { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
