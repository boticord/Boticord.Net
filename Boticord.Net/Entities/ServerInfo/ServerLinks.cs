using Newtonsoft.Json;

namespace Boticord.Net.Entities;

/// <summary>
/// Class containing links from server's profile
/// </summary>
public class ServerLinks
{
    [JsonProperty("invite")]
    public string? Invite { get; init; }

    [JsonProperty("site")]
    public string? Site { get; init; }

    [JsonProperty("youtube")]
    public string? Youtube { get; init; }

    [JsonProperty("twitch")]
    public string? Twitch { get; init; }

    [JsonProperty("steam")]
    public string? Steam { get; init; }

    [JsonProperty("vk")]
    public string? VK { get; init; }
}