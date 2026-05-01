using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SmartApiary.Application.Interfaces;

namespace SmartApiary.Infrastructure.Services
{
    internal class ImageResizerService(ILogger<ImageResizerService> logger) : IImageResizer
    {
        public async Task<byte[]> ResizeAsync(byte[] imageData, int width, int height, CancellationToken ct = default)
        {
            try
            {
                using var image = Image.Load(imageData);

                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(width, height),
                    Mode = ResizeMode.Crop
                }));

                using var outputStream = new MemoryStream();
                await image.SaveAsync(outputStream, new JpegEncoder { Quality = 80 }, ct);

                return outputStream.ToArray();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[IMAGE] Error resizing image to {Width}x{Height}.", width, height);
                throw;
            }
        }
    }
}
