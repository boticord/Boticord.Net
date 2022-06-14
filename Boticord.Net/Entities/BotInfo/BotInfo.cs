using Newtonsoft.Json;

namespace Boticord.Net.Entities;

public class BotInfo
{
    [JsonProperty("id")]
    public string Id { get; init; }

    [JsonProperty("shortCode")]
    public string? ShortCode { get; init; }

    [JsonProperty("links")]
    public string[] Links { get; init; }

    [JsonProperty("information")]
    public BotInformation Information { get; init; }

}