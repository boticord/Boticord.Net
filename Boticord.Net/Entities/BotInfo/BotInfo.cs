using Newtonsoft.Json;

namespace Boticord.Net.Entities;

/// <summary>
/// Class representing general information about a bot
/// </summary>
public class BotInfo
{
    [JsonProperty("id")]
    protected string _id { get; init; }

    [JsonIgnore] 
    public ulong Id => ulong.Parse(_id);

    [JsonProperty("shortCode")]
    public string? ShortCode { get; init; }

    [JsonProperty("links")]
    public string[] Links { get; init; }

    [JsonProperty("information")]
    public BotInformation Information { get; init; }

}