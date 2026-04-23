using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces.Storage;

namespace SmartApiary.Infrastructure.Persistence.AzureBlob;

internal class AzureBlobStorage(
    BlobServiceClient blobServiceClient,
    ILogger<AzureBlobStorage> logger) : IFileStorage
{
    private static readonly FileExtensionContentTypeProvider _contentTypeProvider = new();

    public async Task<string> UploadAsync(
        string containerName,
        string fileName,
        Stream content,
        string contentType,
        CancellationToken ct = default)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: ct);

        var blobClient = containerClient.GetBlobClient(fileName);

        var blobOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType ?? GetContentType(fileName)
            }
        };

        await blobClient.UploadAsync(content, blobOptions, ct);

        logger.LogInformation("Uploaded blob: {Container}/{FileName}", containerName, fileName);

        return blobClient.Uri.ToString();
    }

    public async Task<Stream> DownloadAsync(
        string containerName,
        string fileName,
        CancellationToken ct = default)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        var response = await blobClient.DownloadStreamingAsync(cancellationToken: ct);
        return response.Value.Content;
    }

    public async Task DeleteAsync(
        string containerName,
        string fileName,
        CancellationToken ct = default)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.DeleteIfExistsAsync(
            DeleteSnapshotsOption.IncludeSnapshots,
            cancellationToken: ct);

        logger.LogInformation("Deleted blob: {Container}/{FileName}", containerName, fileName);
    }

    private static string GetContentType(string path)
    {
        if (_contentTypeProvider.TryGetContentType(path, out var contentType))
            return contentType!;

        return "application/octet-stream";
    }
}