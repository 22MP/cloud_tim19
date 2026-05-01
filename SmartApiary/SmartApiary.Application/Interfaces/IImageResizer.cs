namespace SmartApiary.Application.Interfaces
{
    public interface IImageResizer
    {
        Task<byte[]> ResizeAsync(byte[] imageData, int width, int height, CancellationToken ct = default);
    }
}
