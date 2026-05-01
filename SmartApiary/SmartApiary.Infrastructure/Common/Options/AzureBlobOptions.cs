namespace SmartApiary.Infrastructure.Common.Options
{
    internal class AzureBlobOptions
    {
        public string ConnectionString { get; init; } = string.Empty;
        public string ApiaryImagesContainer { get; init; } = "apiary-images";
    }
}