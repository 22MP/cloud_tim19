using SmartApiary.Domain.Enums.Apiaries;

namespace SmartApiary.Application.Features.Hives.DTOs
{
    public class HiveDto
    {
        public string Id { get; init; } = string.Empty;
        public string ApiaryId { get; init; } = string.Empty;
        public string Label { get; init; } = string.Empty;
        public HiveType Type { get; init; }
        public string? BoxColor { get; init; }
        public int? QueenAge { get; init; }
        public string? Note { get; init; }
        public bool IsPaired { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
