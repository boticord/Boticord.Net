namespace Boticord.Net.Core;

/// <summary>
/// Class representing statistics of the bots
/// </summary>
public class BotStats
{
    [JsonProperty("servers")]
    public int Servers { get; init; }

    [JsonProperty("Shards")]
    public int Shards { get; init; }

    [JsonProperty("users")]
    public int Users { get; init; }
}