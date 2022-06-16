using Newtonsoft.Json;

namespace Boticord.Net.Entities;

/// <summary>
/// Ok
/// </summary>
public class OkResponse
{
    [JsonProperty("ok")]
    public bool Ok { get; init; }
}