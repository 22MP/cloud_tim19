using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartApiary.Application.Common;
using SmartApiary.Application.Interfaces.Storage;
using SmartApiary.Infrastructure.Common.Options;

namespace SmartApiary.Infrastructure.Persistence.AzureBlob.Storages
{
    internal class ApiaryImageStorage(
        BlobServiceClient blobServiceClient,
        ILogger<ApiaryImageStorage> logger,
        IOptions<AzureBlobOptions> options)
        : AzureBlobStorage<ApiaryImageMetadata>(
            blobServiceClient.GetBlobContainerClient(options.Value.ApiaryImagesContainer),
            logger),
          IApiaryImageStorage
    {
        public override string GetBlobPath(ApiaryImageMetadata metadata)
        {
            var folder = metadata.IsThumbnail ? "thumbnails" : "originals";
            return $"{folder}/apiary-{metadata.ApiaryId}.jpg";
        }

        public string GetBlobUrl(ApiaryImageMetadata metadata)
            => $"{_containerClient.Uri}/{GetBlobPath(metadata)}";

        public async Task DeleteAllAsync(string apiaryId, CancellationToken ct = default)
        {
            foreach (var isThumbnail in new[] { false, true })
            {
                var metadata = new ApiaryImageMetadata
                {
                    ApiaryId = apiaryId,
                    IsThumbnail = isThumbnail
                };
                await DeleteAsync(metadata, ct);
            }

            logger.LogInformation("[BLOB] Deleted all images for apiary {ApiaryId}.", apiaryId);
        }
    }
}