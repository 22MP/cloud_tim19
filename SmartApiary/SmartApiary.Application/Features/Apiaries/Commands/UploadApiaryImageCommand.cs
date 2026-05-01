using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Common;
using SmartApiary.Application.Interfaces;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Storage;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.ValueObjects;

namespace SmartApiary.Application.Features.Apiaries.Commands.UploadApiaryImage
{
    public record UploadApiaryImageCommand : IRequest<Result<UploadApiaryImageResult>>
    {
        public string ApiaryId { get; init; } = string.Empty;
        public byte[] ImageData { get; init; } = [];
        public string ContentType { get; init; } = string.Empty;
    }

    public record UploadApiaryImageResult(string ImageUrl, string ThumbnailUrl);

    public class UploadApiaryImageValidator : AbstractValidator<UploadApiaryImageCommand>
    {
        private static readonly string[] AllowedContentTypes = ["image/jpeg", "image/png", "image/webp"];
        private const long MaxFileSizeBytes = 10 * 1024 * 1024;

        public UploadApiaryImageValidator()
        {
            RuleFor(x => x.ApiaryId)
                .NotEmpty().WithMessage("Apiary ID is required.")
                .Must(id => Guid.TryParse(id, out _)).WithMessage("Invalid apiary ID format.");

            RuleFor(x => x.ImageData)
                .NotEmpty().WithMessage("Image data is required.")
                .Must(data => data.Length <= MaxFileSizeBytes)
                .WithMessage("Image size must not exceed 10 MB.");

            RuleFor(x => x.ContentType)
                .NotEmpty().WithMessage("Content type is required.")
                .Must(ct => AllowedContentTypes.Contains(ct))
                .WithMessage("Only JPEG, PNG and WebP images are allowed.");
        }
    }

    internal class UploadApiaryImageHandler(
        IApiaryRepository apiaryRepository,
        IApiaryImageStorage imageStorage,
        IImageResizer imageResizer,
        ICurrentUserService currentUserService,
        ILogger<UploadApiaryImageHandler> logger) : IRequestHandler<UploadApiaryImageCommand, Result<UploadApiaryImageResult>>
    {
        private const int ThumbnailWidth = 150;
        private const int ThumbnailHeight = 150;

        public async Task<Result<UploadApiaryImageResult>> Handle(UploadApiaryImageCommand request, CancellationToken ct)
        {
            var idResult = EntityId.Create(request.ApiaryId);
            if (idResult.IsFailure)
                return Result<UploadApiaryImageResult>.Failure(idResult.Error!.Message, ErrorType.Validation);

            var apiary = await apiaryRepository.GetByIdAsync(idResult.Value, ct);
            if (apiary is null)
                return Result<UploadApiaryImageResult>.Failure("Apiary not found.", ErrorType.NotFound);

            if (!apiary.IsOwnedBy(currentUserService.UserId))
                return Result<UploadApiaryImageResult>.Failure("Access denied.", ErrorType.Unauthorized);

            try
            {
                var originalMetadata = new ApiaryImageMetadata { ApiaryId = request.ApiaryId, IsThumbnail = false };
                var thumbnailMetadata = new ApiaryImageMetadata { ApiaryId = request.ApiaryId, IsThumbnail = true };

                // Upload originala
                await imageStorage.SaveAsync(
                    new FileData<ApiaryImageMetadata> { Metadata = originalMetadata, Content = request.ImageData }, ct);

                
                var thumbnailData = await imageResizer.ResizeAsync(request.ImageData, ThumbnailWidth, ThumbnailHeight, ct);
                await imageStorage.SaveAsync(
                    new FileData<ApiaryImageMetadata> { Metadata = thumbnailMetadata, Content = thumbnailData }, ct);

                
                var imageUrl = imageStorage.GetBlobUrl(originalMetadata);
                var thumbnailUrl = imageStorage.GetBlobUrl(thumbnailMetadata);

                apiary.SetImages(imageUrl, thumbnailUrl);
                await apiaryRepository.UpdateAsync(apiary, ct);

                logger.LogInformation("[APIARY] Image uploaded for apiary {Id}.", request.ApiaryId);

                return Result<UploadApiaryImageResult>.Success(new UploadApiaryImageResult(imageUrl, thumbnailUrl));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[APIARY] Error uploading image for apiary {Id}.", request.ApiaryId);
                return Result<UploadApiaryImageResult>.Failure("Failed to upload image.", ErrorType.Failure);
            }
        }
    }
}