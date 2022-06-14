using Newtonsoft.Json;

namespace Boticord.Net.Entities;

public class BotStats
{
    [JsonProperty("servers")]
    public int Servers { get; init; }

    [JsonProperty("Shards")]
    public int Shards { get; init; }

    [JsonProperty("users")]
    public int Users { get; init; }
}