using SmartApiary.Application.Features.Apiaries.DTOs;
using SmartApiary.Application.Interfaces;
using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Features.Apiaries.Mappers
{
    internal class ApiaryMapper : IMapper<Apiary, ApiaryDto>
    {
        public ApiaryDto Map(Apiary source)
        {
            return new ApiaryDto
            {
                Id = source.Id.Value,
                Name = source.Name,
                Latitude = source.Latitude,
                Longitude = source.Longitude,
                Description = source.Description,
                ImageUrl = source.ImageUrl,
                ThumbnailUrl = source.ThumbnailUrl,
                HiveCount = source.Hives.Count,
                CreatedAt = source.CreatedAt
            };
        }
    }
}
