using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.Enums.Farmer;
using SmartApiary.Domain.Events;
using SmartApiary.Domain.ValueObjects;

namespace SmartApiary.Domain.Models
{
    // AGGREGATE ROOT
    public class Plot : AggregateRoot
    {
        public EntityId Id { get; private set; } = null!;
        public string OwnerId { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // CHILD ENTITIES
        private readonly List<SprayingSchedule> _sprayingSchedules = [];
        public IReadOnlyCollection<SprayingSchedule> SprayingSchedules => _sprayingSchedules.AsReadOnly();

        private Culture? _culture;
        public Culture? Culture => _culture;

        private Plot(
            EntityId id,
            string ownerId,
            string name,
            double latitude,
            double longitude,
            DateTime createdAt)
        {
            Id = id;
            OwnerId = ownerId;
            Name = name;
            Latitude = latitude;
            Longitude = longitude;
            CreatedAt = createdAt;
        }

        #region Factory Methods

        public static Result<Plot> Create(
            string ownerId,
            string name,
            double latitude,
            double longitude,
            DateTime createdAt)
        {
            if (string.IsNullOrWhiteSpace(ownerId))
                return Result<Plot>.Failure("Owner is required.", ErrorType.Validation);

            if (string.IsNullOrWhiteSpace(name))
                return Result<Plot>.Failure("Plot name is required.", ErrorType.Validation);

            if (latitude < -90 || latitude > 90)
                return Result<Plot>.Failure("Latitude must be between -90 and 90.", ErrorType.Validation);

            if (longitude < -180 || longitude > 180)
                return Result<Plot>.Failure("Longitude must be between -180 and 180.", ErrorType.Validation);

            var plot = new Plot(EntityId.New(), ownerId, name, latitude, longitude, createdAt);

            plot.AddDomainEvent(new PlotCreatedDomainEvent(plot.Id, ownerId, createdAt));

            return Result<Plot>.Success(plot);
        }

        public static Result<Plot> Load(
            string id,
            string ownerId,
            string name,
            double latitude,
            double longitude,
            DateTime createdAt,
            Culture? culture = null,
            IEnumerable<SprayingSchedule>? sprayingSchedules = null)
        {
            var idResult = EntityId.Create(id);
            if (idResult.IsFailure)
                return Result<Plot>.Failure(idResult.Error!.Message, ErrorType.Validation);

            var plot = new Plot(idResult.Value, ownerId, name, latitude, longitude, createdAt)
            {
                _culture = culture
            };

            if (sprayingSchedules is not null)
                plot._sprayingSchedules.AddRange(sprayingSchedules);

            return Result<Plot>.Success(plot);
        }

        #endregion

        #region Domain Logic

        public Result UpdateLocation(string name, double latitude, double longitude)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure("Plot name is required.", ErrorType.Validation);

            if (latitude < -90 || latitude > 90)
                return Result.Failure("Latitude must be between -90 and 90.", ErrorType.Validation);

            if (longitude < -180 || longitude > 180)
                return Result.Failure("Longitude must be between -180 and 180.", ErrorType.Validation);

            Name = name;
            Latitude = latitude;
            Longitude = longitude;

            return Result.Success();
        }

        public Result SetCulture(Culture culture)
        {
            // Replacing an old culture is allowed (after harvest)
            _culture = culture;
            return Result.Success();
        }

        public Result RemoveCulture()
        {
            if (_culture is null)
                return Result.Failure("No culture is currently set on this plot.", ErrorType.Validation);

            _culture = null;
            return Result.Success();
        }

        public Result<SprayingSchedule> ScheduleSpraying(
            DateTime scheduledAt,
            int durationHours,
            string? preparation,
            DateTime now)
        {
            var scheduleResult = SprayingSchedule.Create(Id.Value, scheduledAt, durationHours, preparation, now);
            if (scheduleResult.IsFailure)
                return Result<SprayingSchedule>.Failure(scheduleResult.Error!.Message, scheduleResult.Error!.Type);

            var schedule = scheduleResult.Value;
            _sprayingSchedules.Add(schedule);

            AddDomainEvent(new SprayingScheduledDomainEvent(
                schedule.Id, Id, OwnerId, scheduledAt, durationHours, Latitude, Longitude, now));

            return Result<SprayingSchedule>.Success(schedule);
        }

        public Result CancelSpraying(string scheduleId, DateTime now)
        {
            var idResult = EntityId.Create(scheduleId);
            if (idResult.IsFailure)
                return Result.Failure(idResult.Error!.Message, ErrorType.Validation);

            var schedule = _sprayingSchedules.FirstOrDefault(s => s.Id.Value == scheduleId);
            if (schedule is null)
                return Result.Failure("Spraying schedule not found.", ErrorType.NotFound);

            var cancelResult = schedule.Cancel();
            if (cancelResult.IsFailure)
                return cancelResult;

            AddDomainEvent(new SprayingCancelledDomainEvent(schedule.Id, Id, OwnerId, now));

            return Result.Success();
        }

        public Result RescheduleSpraying(string scheduleId, DateTime newScheduledAt, int newDurationHours, DateTime now)
        {
            var schedule = _sprayingSchedules.FirstOrDefault(s => s.Id.Value == scheduleId);
            if (schedule is null)
                return Result.Failure("Spraying schedule not found.", ErrorType.NotFound);

            var rescheduleResult = schedule.Reschedule(newScheduledAt, newDurationHours, now);
            if (rescheduleResult.IsFailure)
                return rescheduleResult;

            AddDomainEvent(new SprayingRescheduledDomainEvent(
                schedule.Id, Id, OwnerId, newScheduledAt, Latitude, Longitude, now));

            return Result.Success();
        }

        public bool IsOwnedBy(string userId) => OwnerId == userId;

        #endregion
    }
}
