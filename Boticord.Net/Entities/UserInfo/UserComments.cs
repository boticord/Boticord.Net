using Newtonsoft.Json;

namespace Boticord.Net.Entities;

public class UserComments
{
    [JsonProperty("bots")]
    public Comment[] Bots { get; init; }

    [JsonProperty("servers")]
    public Comment[] Servers { get; init; }
}