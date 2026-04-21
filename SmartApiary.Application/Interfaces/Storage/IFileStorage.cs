namespace SmartApiary.Application.Interfaces.Storage;

public interface IFileStorage
{
    Task<string> UploadAsync(string containerName, string fileName, Stream content, string contentType, CancellationToken ct = default);
    Task<Stream> DownloadAsync(string containerName, string fileName, CancellationToken ct = default);
    Task DeleteAsync(string containerName, string fileName, CancellationToken ct = default);
}