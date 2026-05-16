using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.Enums.Farmer;
using SmartApiary.Domain.ValueObjects;

namespace SmartApiary.Domain.Models
{
    // CHILD ENTITY (belongs to Plot aggregate)
    public class Culture
    {
        public EntityId Id { get; private set; } = null!;
        public EntityId PlotId { get; private set; } = null!;
        public CultureType Type { get; private set; }
        public DateTime BloomingStart { get; private set; }
        public DateTime BloomingEnd { get; private set; }
        public string? AdditionalInfo { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Culture(
            EntityId id,
            EntityId plotId,
            CultureType type,
            DateTime bloomingStart,
            DateTime bloomingEnd,
            string? additionalInfo,
            DateTime createdAt)
        {
            Id = id;
            PlotId = plotId;
            Type = type;
            BloomingStart = bloomingStart;
            BloomingEnd = bloomingEnd;
            AdditionalInfo = additionalInfo;
            CreatedAt = createdAt;
        }

        #region Factory Methods

        public static Result<Culture> Create(
            string plotId,
            CultureType type,
            DateTime bloomingStart,
            DateTime bloomingEnd,
            string? additionalInfo,
            DateTime createdAt)
        {
            var plotIdResult = EntityId.Create(plotId);
            if (plotIdResult.IsFailure)
                return Result<Culture>.Failure(plotIdResult.Error!.Message, ErrorType.Validation);

            if (!Enum.IsDefined(typeof(CultureType), type) || type == CultureType.Unknown)
                return Result<Culture>.Failure("Invalid culture type.", ErrorType.Validation);

            if (bloomingEnd <= bloomingStart)
                return Result<Culture>.Failure("Blooming end date must be after start date.", ErrorType.Validation);

            var culture = new Culture(
                EntityId.New(),
                plotIdResult.Value,
                type,
                bloomingStart,
                bloomingEnd,
                additionalInfo,
                createdAt);

            return Result<Culture>.Success(culture);
        }

        public static Result<Culture> Load(
            string id,
            string plotId,
            CultureType type,
            DateTime bloomingStart,
            DateTime bloomingEnd,
            string? additionalInfo,
            DateTime createdAt)
        {
            var idResult = EntityId.Create(id);
            if (idResult.IsFailure)
                return Result<Culture>.Failure(idResult.Error!.Message, ErrorType.Validation);

            var plotIdResult = EntityId.Create(plotId);
            if (plotIdResult.IsFailure)
                return Result<Culture>.Failure(plotIdResult.Error!.Message, ErrorType.Validation);

            var culture = new Culture(
                idResult.Value,
                plotIdResult.Value,
                type,
                bloomingStart,
                bloomingEnd,
                additionalInfo,
                createdAt);

            return Result<Culture>.Success(culture);
        }

        #endregion
    }
}
