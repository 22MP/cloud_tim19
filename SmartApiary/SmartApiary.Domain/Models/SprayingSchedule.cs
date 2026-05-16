using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.Enums.Farmer;
using SmartApiary.Domain.Events;
using SmartApiary.Domain.ValueObjects;

namespace SmartApiary.Domain.Models
{
    // CHILD ENTITY (belongs to Plot aggregate)
    public class SprayingSchedule
    {
        public EntityId Id { get; private set; } = null!;
        public EntityId PlotId { get; private set; } = null!;
        public DateTime ScheduledAt { get; private set; }
        public int DurationHours { get; private set; }
        public string? Preparation { get; private set; }
        public SprayingStatus Status { get; private set; }
        public int NotifiedBeekeeperCount { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private SprayingSchedule(
            EntityId id,
            EntityId plotId,
            DateTime scheduledAt,
            int durationHours,
            string? preparation,
            SprayingStatus status,
            int notifiedBeekeeperCount,
            DateTime createdAt)
        {
            Id = id;
            PlotId = plotId;
            ScheduledAt = scheduledAt;
            DurationHours = durationHours;
            Preparation = preparation;
            Status = status;
            NotifiedBeekeeperCount = notifiedBeekeeperCount;
            CreatedAt = createdAt;
        }

        #region Factory Methods

        public static Result<SprayingSchedule> Create(
            string plotId,
            DateTime scheduledAt,
            int durationHours,
            string? preparation,
            DateTime createdAt)
        {
            var plotIdResult = EntityId.Create(plotId);
            if (plotIdResult.IsFailure)
                return Result<SprayingSchedule>.Failure(plotIdResult.Error!.Message, ErrorType.Validation);

            if (scheduledAt <= createdAt)
                return Result<SprayingSchedule>.Failure("Scheduled date must be in the future.", ErrorType.Validation);

            if (durationHours <= 0)
                return Result<SprayingSchedule>.Failure("Duration must be at least 1 hour.", ErrorType.Validation);

            var schedule = new SprayingSchedule(
                EntityId.New(),
                plotIdResult.Value,
                scheduledAt,
                durationHours,
                preparation,
                SprayingStatus.Scheduled,
                0,
                createdAt);

            return Result<SprayingSchedule>.Success(schedule);
        }

        public static Result<SprayingSchedule> Load(
            string id,
            string plotId,
            DateTime scheduledAt,
            int durationHours,
            string? preparation,
            SprayingStatus status,
            int notifiedBeekeeperCount,
            DateTime createdAt)
        {
            var idResult = EntityId.Create(id);
            if (idResult.IsFailure)
                return Result<SprayingSchedule>.Failure(idResult.Error!.Message, ErrorType.Validation);

            var plotIdResult = EntityId.Create(plotId);
            if (plotIdResult.IsFailure)
                return Result<SprayingSchedule>.Failure(plotIdResult.Error!.Message, ErrorType.Validation);

            var schedule = new SprayingSchedule(
                idResult.Value,
                plotIdResult.Value,
                scheduledAt,
                durationHours,
                preparation,
                status,
                notifiedBeekeeperCount,
                createdAt);

            return Result<SprayingSchedule>.Success(schedule);
        }

        #endregion

        #region Domain Logic

        public Result Reschedule(DateTime newScheduledAt, int newDurationHours, DateTime now)
        {
            if (Status == SprayingStatus.Cancelled)
                return Result.Failure("Cannot reschedule a cancelled spraying.", ErrorType.Validation);

            if (Status == SprayingStatus.Completed)
                return Result.Failure("Cannot reschedule a completed spraying.", ErrorType.Validation);

            if (newScheduledAt <= now)
                return Result.Failure("New scheduled date must be in the future.", ErrorType.Validation);

            if (newDurationHours <= 0)
                return Result.Failure("Duration must be at least 1 hour.", ErrorType.Validation);

            ScheduledAt = newScheduledAt;
            DurationHours = newDurationHours;

            return Result.Success();
        }

        public Result Cancel()
        {
            if (Status == SprayingStatus.Cancelled)
                return Result.Failure("Spraying is already cancelled.", ErrorType.Conflict);

            if (Status == SprayingStatus.Completed)
                return Result.Failure("Cannot cancel a completed spraying.", ErrorType.Validation);

            Status = SprayingStatus.Cancelled;
            return Result.Success();
        }

        public Result Complete()
        {
            if (Status == SprayingStatus.Cancelled)
                return Result.Failure("Cannot complete a cancelled spraying.", ErrorType.Validation);

            if (Status == SprayingStatus.Completed)
                return Result.Failure("Spraying is already completed.", ErrorType.Conflict);

            Status = SprayingStatus.Completed;
            return Result.Success();
        }

        public void SetNotifiedBeekeeperCount(int count)
        {
            if (count >= 0)
                NotifiedBeekeeperCount = count;
        }

        public bool CanBeModified => Status == SprayingStatus.Scheduled;

        #endregion
    }
}
