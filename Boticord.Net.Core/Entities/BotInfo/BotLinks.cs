namespace Boticord.Net.Core;

/// <summary>
/// Class representing information about links in bot's profile
/// </summary>
public class BotLinks
{
    [JsonProperty("discord")]
    public string? Discord { get; init; }

    [JsonProperty("github")]
    public string? GitHub { get; init; }

    [JsonProperty("site")]
    public string? Site { get; init; }

}