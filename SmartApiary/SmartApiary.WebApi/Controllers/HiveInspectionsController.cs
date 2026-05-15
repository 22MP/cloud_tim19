using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartApiary.Application.Interfaces;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Models;
using SmartApiary.Domain.ValueObjects;

namespace SmartApiary.WebApi.Controllers
{
    [Route("api/hives/{hiveId}/inspections")]
    [ApiController]
    [Authorize(Roles = "Beekeeper")]
    public class HiveInspectionsController : ControllerBase
    {
        private readonly IHiveInspectionRepository _repo;
        private readonly IHiveRepository _hiveRepo;
        private readonly IApiaryRepository _apiaryRepo;
        private readonly ICurrentUserService _current;

        public HiveInspectionsController(
            IHiveInspectionRepository repo,
            IHiveRepository hiveRepo,
            IApiaryRepository apiaryRepo,
            ICurrentUserService current)
        {
            _repo = repo;
            _hiveRepo = hiveRepo;
            _apiaryRepo = apiaryRepo;
            _current = current;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string hiveId, CancellationToken ct)
        {
            var hiveIdResult = EntityId.Create(hiveId);
            if (hiveIdResult.IsFailure)
                return BadRequest("Invalid hive ID.");

            var hive = await _hiveRepo.GetByIdAsync(hiveIdResult.Value, ct);
            if (hive == null)
                return NotFound("Hive not found.");

            var apiary = await _apiaryRepo.GetByIdAsync(hive.ApiaryId, ct);
            if (apiary == null || !apiary.IsOwnedBy(_current.UserId))
                return Unauthorized();

            var data = await _repo.GetByHiveAsync(hiveIdResult.Value, ct);

            return Ok(data.Select(x => new
            {
                id = x.Id.Value,
                hiveId = x.HiveId.Value,
                inspectionDate = x.InspectionDate,
                bottomBoardColor = x.BottomBoardColor,
                honeyFrames = x.HoneyFrames,
                honeyKg = x.HoneyKg,
                broodFrames = x.BroodFrames,
                queenPresent = x.QueenPresent,
                notes = x.Notes,
                createdAt = x.CreatedAt
            }));
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            string hiveId,
            [FromBody] CreateInspectionRequest request,
            CancellationToken ct)
        {
            var hiveIdResult = EntityId.Create(hiveId);
            if (hiveIdResult.IsFailure)
                return BadRequest("Invalid hive ID.");

            var hive = await _hiveRepo.GetByIdAsync(hiveIdResult.Value, ct);
            if (hive == null)
                return NotFound("Hive not found.");

            var apiary = await _apiaryRepo.GetByIdAsync(hive.ApiaryId, ct);
            if (apiary == null || !apiary.IsOwnedBy(_current.UserId))
                return Unauthorized();

            var inspectionResult = HiveInspection.Create(
                hiveId,
                request.InspectionDate,
                request.BottomBoardColor,
                request.HoneyFrames,
                request.HoneyKg,
                request.BroodFrames,
                request.QueenPresent,
                request.Notes,
                DateTime.UtcNow
            );

            if (inspectionResult.IsFailure)
                return BadRequest(inspectionResult.Error?.Message);

            await _repo.AddAsync(inspectionResult.Value, ct);

            return Ok();
        }

        [HttpPut("{inspectionId}")]
        public async Task<IActionResult> Update(
            string hiveId,
            string inspectionId,
            [FromBody] CreateInspectionRequest request,
            CancellationToken ct)
        {
            var inspectionIdResult = EntityId.Create(inspectionId);
            if (inspectionIdResult.IsFailure)
                return BadRequest("Invalid inspection ID.");

            var inspection = await _repo.GetByIdAsync(inspectionIdResult.Value, ct);
            if (inspection == null)
                return NotFound("Inspection not found.");

            var updateResult = inspection.Update(
                request.InspectionDate,
                request.BottomBoardColor,
                request.HoneyFrames,
                request.HoneyKg,
                request.BroodFrames,
                request.QueenPresent,
                request.Notes
            );

            if (updateResult.IsFailure)
                return BadRequest(updateResult.Error?.Message);

            await _repo.UpdateAsync(inspection, ct);

            return Ok();
        }

        [HttpDelete("{inspectionId}")]
        public async Task<IActionResult> Delete(
            string hiveId,
            string inspectionId,
            CancellationToken ct)
        {
            var inspectionIdResult = EntityId.Create(inspectionId);
            if (inspectionIdResult.IsFailure)
                return BadRequest("Invalid inspection ID.");

            await _repo.DeleteAsync(inspectionIdResult.Value, ct);

            return Ok();
        }
    }

    public record CreateInspectionRequest(
        DateTime InspectionDate,
        string BottomBoardColor,
        int HoneyFrames,
        double HoneyKg,
        int BroodFrames,
        bool QueenPresent,
        string? Notes);
}