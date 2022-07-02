namespace Boticord.Net.Core;

/// <summary>
/// Class representing information about user's bot
/// </summary>
public class UsersBot
{
    [JsonProperty("id")]
    protected string _id { get; init; }

    [JsonIgnore]
    public ulong Id => ulong.Parse(_id);

    [JsonProperty("shortCode")]
    public string? ShortCode { get; init; }
}