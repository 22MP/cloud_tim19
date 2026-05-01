namespace SmartApiary.WebApi.DTOs
{
    public record UpdateApiaryRequest(
         string Name,
         double Latitude,
         double Longitude,
         string? Description);
}
