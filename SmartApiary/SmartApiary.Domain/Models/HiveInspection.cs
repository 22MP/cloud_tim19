using SmartApiary.Domain.Common;
using SmartApiary.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Domain.Models
{
    public class HiveInspection
    {
        public EntityId Id { get; private set; }
        public EntityId HiveId { get; private set; }

        public DateTime InspectionDate { get; private set; }
        public string BottomBoardColor { get; private set; } = string.Empty;

        public int HoneyFrames { get; private set; }
        public double HoneyKg { get; private set; }

        public int BroodFrames { get; private set; }
        public bool QueenPresent { get; private set; }

        public string? Notes { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private HiveInspection() { }

        private HiveInspection(
            EntityId id,
            EntityId hiveId,
            DateTime inspectionDate,
            string bottomBoardColor,
            int honeyFrames,
            double honeyKg,
            int broodFrames,
            bool queenPresent,
            string? notes,
            DateTime createdAt)
        {
            Id = id;
            HiveId = hiveId;
            InspectionDate = inspectionDate;
            BottomBoardColor = bottomBoardColor;
            HoneyFrames = honeyFrames;
            HoneyKg = honeyKg;
            BroodFrames = broodFrames;
            QueenPresent = queenPresent;
            Notes = notes;
            CreatedAt = createdAt;
        }

        public static Result<HiveInspection> Create(
            string hiveId,
            DateTime inspectionDate,
            string bottomBoardColor,
            int honeyFrames,
            double honeyKg,
            int broodFrames,
            bool queenPresent,
            string? notes,
            DateTime createdAt)
        {
            var hiveIdResult = EntityId.Create(hiveId);
            if (hiveIdResult.IsFailure)
                return Result<HiveInspection>.Failure(hiveIdResult.Error!.Message);

            if (string.IsNullOrWhiteSpace(bottomBoardColor))
                return Result<HiveInspection>.Failure("Bottom board color is required.");

            if (honeyFrames < 0 || broodFrames < 0)
                return Result<HiveInspection>.Failure("Frame counts cannot be negative.");

            if (honeyKg < 0)
                return Result<HiveInspection>.Failure("Honey kg cannot be negative.");

            return Result<HiveInspection>.Success(
                new HiveInspection(
                    EntityId.New(),
                    hiveIdResult.Value,
                    inspectionDate,
                    bottomBoardColor,
                    honeyFrames,
                    honeyKg,
                    broodFrames,
                    queenPresent,
                    notes,
                    createdAt));
        }

        public Result Update(
            DateTime inspectionDate,
            string bottomBoardColor,
            int honeyFrames,
            double honeyKg,
            int broodFrames,
            bool queenPresent,
            string? notes)
        {
            if (string.IsNullOrWhiteSpace(bottomBoardColor))
                return Result.Failure("Bottom board color is required.");

            if (honeyFrames < 0 || broodFrames < 0)
                return Result.Failure("Frame counts cannot be negative.");

            if (honeyKg < 0)
                return Result.Failure("Honey kg cannot be negative.");

            InspectionDate = inspectionDate;
            BottomBoardColor = bottomBoardColor;
            HoneyFrames = honeyFrames;
            HoneyKg = honeyKg;
            BroodFrames = broodFrames;
            QueenPresent = queenPresent;
            Notes = notes;

            return Result.Success();
        }
    }
}
