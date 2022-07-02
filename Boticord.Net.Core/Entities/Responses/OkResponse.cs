namespace Boticord.Net.Core;

/// <summary>
/// Ok
/// </summary>
public class OkResponse
{
    [JsonProperty("ok")]
    public bool Ok { get; init; }
}