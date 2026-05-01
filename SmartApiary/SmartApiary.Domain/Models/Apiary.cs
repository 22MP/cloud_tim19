using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.Enums.Apiaries;
using SmartApiary.Domain.Events;
using SmartApiary.Domain.ValueObjects;

namespace SmartApiary.Domain.Models
{
    // AGGREGATE ROOT
    public class Apiary : AggregateRoot
    {
        public EntityId Id { get; private set; }
        public string OwnerId { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public string? Description { get; private set; }
        public string? ImageUrl { get; private set; }
        public string? ThumbnailUrl { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // CHILD ENTITIES
        private readonly List<Hive> _hives = [];
        public IReadOnlyCollection<Hive> Hives => _hives.AsReadOnly();

        private Apiary(
            EntityId id,
            string ownerId,
            string name,
            double latitude,
            double longitude,
            string? description,
            DateTime createdAt)
        {
            Id = id;
            OwnerId = ownerId;
            Name = name;
            Latitude = latitude;
            Longitude = longitude;
            Description = description;
            CreatedAt = createdAt;
        }

        #region Factory Methods

        public static Result<Apiary> Create(
            string ownerId,
            string name,
            double latitude,
            double longitude,
            string? description,
            DateTime createdAt)
        {
            if (string.IsNullOrWhiteSpace(ownerId))
                return Result<Apiary>.Failure("Owner is required.", ErrorType.Validation);

            if (string.IsNullOrWhiteSpace(name))
                return Result<Apiary>.Failure("Apiary name is required.", ErrorType.Validation);

            if (latitude < -90 || latitude > 90)
                return Result<Apiary>.Failure("Latitude must be between -90 and 90.", ErrorType.Validation);

            if (longitude < -180 || longitude > 180)
                return Result<Apiary>.Failure("Longitude must be between -180 and 180.", ErrorType.Validation);

            var apiary = new Apiary(EntityId.New(), ownerId, name, latitude, longitude, description, createdAt);

            apiary.AddDomainEvent(new ApiaryCreatedDomainEvent(apiary.Id, ownerId, createdAt));

            return Result<Apiary>.Success(apiary);
        }

        public static Result<Apiary> Load(
            string id,
            string ownerId,
            string name,
            double latitude,
            double longitude,
            string? description,
            string? imageUrl,
            string? thumbnailUrl,
            DateTime createdAt,
            IEnumerable<Hive>? hives = null)
        {
            var idResult = EntityId.Create(id);
            if (idResult.IsFailure)
                return Result<Apiary>.Failure(idResult.Error!.Message, ErrorType.Validation);

            var apiary = new Apiary(idResult.Value, ownerId, name, latitude, longitude, description, createdAt)
            {
                ImageUrl = imageUrl,
                ThumbnailUrl = thumbnailUrl
            };

            if (hives is not null)
                apiary._hives.AddRange(hives);

            return Result<Apiary>.Success(apiary);
        }

        #endregion

        #region Domain Logic

        public Result UpdateInfo(string name, double latitude, double longitude, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure("Apiary name is required.", ErrorType.Validation);

            if (latitude < -90 || latitude > 90)
                return Result.Failure("Latitude must be between -90 and 90.", ErrorType.Validation);

            if (longitude < -180 || longitude > 180)
                return Result.Failure("Longitude must be between -180 and 180.", ErrorType.Validation);

            Name = name;
            Latitude = latitude;
            Longitude = longitude;
            Description = description;

            return Result.Success();
        }

        public void SetImages(string imageUrl, string thumbnailUrl)
        {
            ImageUrl = imageUrl;
            ThumbnailUrl = thumbnailUrl;
        }

        public bool IsOwnedBy(string userId) => OwnerId == userId;

        #endregion
    }
}
