using SmartApiary.Application.Common;


namespace SmartApiary.Application.Interfaces.Storage
{
    public interface IApiaryImageStorage : IFileStorage<ApiaryImageMetadata>
    {
        
        string GetBlobUrl(ApiaryImageMetadata metadata);
        Task DeleteAllAsync(string apiaryId, CancellationToken ct = default);
    }
}