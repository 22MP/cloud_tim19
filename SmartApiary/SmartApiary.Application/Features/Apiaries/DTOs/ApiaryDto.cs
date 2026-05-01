namespace SmartApiary.Application.Features.Apiaries.DTOs
{
    public class ApiaryDto
    {
        public string Id { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public string? Description { get; init; }
        public string? ImageUrl { get; init; }
        public string? ThumbnailUrl { get; init; }
        public int HiveCount { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
