using System.Diagnostics.Contracts;

using Newtonsoft.Json;

namespace Boticord.Net.Entities;

public class Comment
{
    [JsonProperty("userID")]
    protected string _userId { get; init; }

    [JsonIgnore]
    public ulong UserId => ulong.Parse(_userId);

    [JsonProperty("text")]
    public string Text { get; init; }

    [JsonProperty("vote")]
    public int Vote { get; init; }

    [JsonProperty("isUpated")]
    public bool IsUpdated { get; init; }

    [JsonProperty("createdAt")]
    protected long? _createdAt { get; init; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt => _createdAt is not null ? DateTimeOffset.FromUnixTimeMilliseconds((long)_createdAt) : null;

    [JsonProperty("updatedAt")]
    protected long? _updatedAt { get; init; }

    [JsonIgnore]
    public DateTimeOffset? UpdatedAt => _updatedAt is not null ? DateTimeOffset.FromUnixTimeMilliseconds((long)_updatedAt) : null;
}