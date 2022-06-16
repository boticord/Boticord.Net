using Newtonsoft.Json;

namespace Boticord.Net.Entities;

/// <summary>
/// Class representing general information about the user
/// </summary>
public class UserInfo
{
    [JsonProperty("id")]
    protected string _id { get; init; }

    [JsonIgnore]
    public ulong Id => ulong.Parse(_id);

    [JsonProperty("status")]
    public string? Status { get; init; }

    [JsonProperty("shortCode")]
    public string? ShortCode { get; init; }

    [JsonProperty("badge")]
    public string? Badge { get; init; }

    [JsonProperty("site")]
    public string? Site { get; init; }

    [JsonProperty("vk")]
    public string? VK { get; init; }

    [JsonProperty("youtube")]
    public string? Youtube { get; init; }

    [JsonProperty("twitch")]
    public string? Twitch { get; init; }

    [JsonProperty("steam")]
    public string? Steam { get; init; }

    [JsonProperty("git")]
    public string? Git { get; init; }

}