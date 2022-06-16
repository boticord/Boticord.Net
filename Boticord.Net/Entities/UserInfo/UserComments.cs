using Newtonsoft.Json;

namespace Boticord.Net.Entities;

/// <summary>
/// Class containing all user comments in two categories
/// </summary>
public class UserComments
{
    [JsonProperty("bots")]
    public Comment[] Bots { get; init; }

    [JsonProperty("servers")]
    public Comment[] Servers { get; init; }
}