using Newtonsoft.Json;

namespace Boticord.Net.Entities;

/// <summary>
/// Class containing detailed information about a server
/// </summary>
public class ServerInformation
{
    [JsonProperty("name")]
    public string Name { get; init; }

    [JsonProperty("avatar")]
    public string Avatar { get; init; }

    [JsonProperty("members")]
    protected int[] _members { get; init; }

    [JsonIgnore]
    public int MembersTotal => _members[0];

    [JsonIgnore]
    public int MembersOnline => _members[1];

    [JsonProperty("owner")]
    protected string? _owner { get; init; }

    [JsonIgnore]
    public ulong? Owner => _owner is not null ? ulong.Parse(_owner) : null;

    [JsonProperty("bumps")]
    public int Bumps { get; init; }

    [JsonProperty("tags")]
    public string[] Tags { get; init; }

    [JsonProperty("links")]
    public ServerLinks Links { get; init; }

    [JsonProperty("shortDescription")]
    public string? ShortDescription { get; init; }

    [JsonProperty("longDescription")]
    public string? Description { get; init; }
    
    [JsonProperty("badge")]
    public string? Badge { get; init; }
}