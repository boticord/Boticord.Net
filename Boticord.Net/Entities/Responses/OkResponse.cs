using Newtonsoft.Json;

namespace Boticord.Net.Entities;

public class OkResponse
{
    [JsonProperty("ok")]
    public bool Ok { get; init; }
}