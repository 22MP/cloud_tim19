using SmartApiary.Application.Features.Hives.DTOs;
using SmartApiary.Application.Interfaces;
using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Features.Hives.Mappers
{
    internal class HiveMapper : IMapper<Hive, HiveDto>
    {
        public HiveDto Map(Hive source)
        {
            return new HiveDto
            {
                Id = source.Id.Value,
                ApiaryId = source.ApiaryId.Value,
                Label = source.Label,
                Type = source.Type,
                BoxColor = source.BoxColor,
                QueenAge = source.QueenAge,
                Note = source.Note,
                IsPaired = false, // will be set by query handler when joining with device data
                CreatedAt = source.CreatedAt
            };
        }
    }
}
