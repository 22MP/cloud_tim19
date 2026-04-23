using SmartApiary.Domain.Models;
using SmartApiary.Infrastructure.Persistence.AzureTable.Common;

namespace SmartApiary.Infrastructure.Persistence.AzureTable.KeyProviders;

internal class UserTableKeyProvider : ITableKeyProvider<User>
{
    public string GetPartitionKey(User model) => "Users";
    public string GetRowKey(User model) => model.Email.ToLower().Trim();
}