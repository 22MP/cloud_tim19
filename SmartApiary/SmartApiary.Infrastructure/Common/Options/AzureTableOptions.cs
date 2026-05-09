namespace SmartApiary.Infrastructure.Common.Options
{
    internal class AzureTableOptions
    {
        public string ConnectionString { get; init; } = string.Empty;
        public string UsersTable { get; set; } = string.Empty;
        public string TelemetrySamplesTable { get; set; } = string.Empty;
    }
}
