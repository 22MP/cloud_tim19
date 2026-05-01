using SmartApiary.Domain.Enums.Apiaries;

namespace SmartApiary.WebApi.DTOs
{
    public record UpdateHiveRequest(
        string Label,
        HiveType Type,
        string? BoxColor,
        int? QueenAge,
        string? Note);
}
