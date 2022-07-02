namespace Boticord.Net.Core;

/// <summary>
/// Class representing a shortened link object
/// </summary>
public class ShortLink
{
    [JsonProperty("id")]
    public int Id { get; init; }

    [JsonProperty("code")]
    public string Code { get; init; }

    [JsonProperty("ownerID")]
    protected string _id { get; init; }

    [JsonIgnore]
    public ulong OwnerId => ulong.Parse(_id);

    [JsonProperty("domain")]
    public string Domain { get; init; }

    [JsonProperty("views")]
    public int Views { get; init; }

    [JsonProperty("link")]
    public string Link { get; init; }

    [JsonProperty("date")]
    protected long _date { get; init; }

    [JsonIgnore]
    public DateTimeOffset Date => DateTimeOffset.FromUnixTimeMilliseconds(_date);
}