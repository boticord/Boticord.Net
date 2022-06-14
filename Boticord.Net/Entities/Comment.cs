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
    public long? CreatedAt { get; init; }

    [JsonProperty("updatedAt")]
    public long? UpdatedAt { get; init; }
}