using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.Enums.Apiaries;
using SmartApiary.Domain.ValueObjects;

namespace SmartApiary.Domain.Models
{
    // CHILD ENTITY (belongs to Apiary aggregate)
    public class Hive
    {
        public EntityId Id { get; private set; }
        public EntityId ApiaryId { get; private set; }
        public string Label { get; private set; } = string.Empty;
        public HiveType Type { get; private set; }
        public string? BoxColor { get; private set; }
        public int? QueenAge { get; private set; }
        public string? Note { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Hive(
            EntityId id,
            EntityId apiaryId,
            string label,
            HiveType type,
            string? boxColor,
            int? queenAge,
            string? note,
            DateTime createdAt)
        {
            Id = id;
            ApiaryId = apiaryId;
            Label = label;
            Type = type;
            BoxColor = boxColor;
            QueenAge = queenAge;
            Note = note;
            CreatedAt = createdAt;
        }

        #region Factory Methods

        public static Result<Hive> Create(
            string apiaryId,
            string label,
            HiveType type,
            string? boxColor,
            int? queenAge,
            string? note,
            DateTime createdAt)
        {
            if (string.IsNullOrWhiteSpace(label))
                return Result<Hive>.Failure("Hive label is required.", ErrorType.Validation);

            if (!Enum.IsDefined(typeof(HiveType), type) || type == HiveType.Unknown)
                return Result<Hive>.Failure("Invalid hive type.", ErrorType.Validation);

            var apiaryIdResult = EntityId.Create(apiaryId);
            if (apiaryIdResult.IsFailure)
                return Result<Hive>.Failure(apiaryIdResult.Error!.Message, ErrorType.Validation);

            if (queenAge.HasValue && queenAge.Value < 0)
                return Result<Hive>.Failure("Queen age cannot be negative.", ErrorType.Validation);

            var hive = new Hive(EntityId.New(), apiaryIdResult.Value, label, type, boxColor, queenAge, note, createdAt);

            return Result<Hive>.Success(hive);
        }

        public static Result<Hive> Load(
            string id,
            string apiaryId,
            string label,
            HiveType type,
            string? boxColor,
            int? queenAge,
            string? note,
            DateTime createdAt)
        {
            var idResult = EntityId.Create(id);
            if (idResult.IsFailure)
                return Result<Hive>.Failure(idResult.Error!.Message, ErrorType.Validation);

            var apiaryIdResult = EntityId.Create(apiaryId);
            if (apiaryIdResult.IsFailure)
                return Result<Hive>.Failure(apiaryIdResult.Error!.Message, ErrorType.Validation);

            var hive = new Hive(idResult.Value, apiaryIdResult.Value, label, type, boxColor, queenAge, note, createdAt);

            return Result<Hive>.Success(hive);
        }

        #endregion

        #region Domain Logic

        public Result Update(string label, HiveType type, string? boxColor, int? queenAge, string? note)
        {
            if (string.IsNullOrWhiteSpace(label))
                return Result.Failure("Hive label is required.", ErrorType.Validation);

            if (!Enum.IsDefined(typeof(HiveType), type) || type == HiveType.Unknown)
                return Result.Failure("Invalid hive type.", ErrorType.Validation);

            if (queenAge.HasValue && queenAge.Value < 0)
                return Result.Failure("Queen age cannot be negative.", ErrorType.Validation);

            Label = label;
            Type = type;
            BoxColor = boxColor;
            QueenAge = queenAge;
            Note = note;

            return Result.Success();
        }

        #endregion
    }
}
