using System.Globalization;
using Newtonsoft.Json;

namespace Boticord.Net.Entities;

/// <summary>
/// Class representing detailed information about the bot
/// </summary>
public class BotInformation
{
    [JsonProperty("developers")]
    protected string[] _developers { get; init; }

    [JsonProperty("bumps")]
    public int Bumps { get; init; }

    [JsonProperty("added")]
    public int Added { get; init; }

    [JsonProperty("prefix")]
    public string Prefix { get; init; }

    [JsonProperty("permissions")]
    public long Permissions { get; init; }

    [JsonProperty("tags")]
    public string[] Tags { get; init; }

    [JsonIgnore]
    public ulong[] Developers
    {
        get
        {
            var output = new ulong[_developers.Length];
            for (var i = 0; i < _developers.Length; i++)
            {
                output[i] = ulong.Parse(_developers[i]);
            }
            return output;
        }
    }

    [JsonProperty("links")]
    public BotLinks Links { get; init; }

    [JsonProperty("library")]
    public string? Library { get; set; }

    [JsonProperty("shortDescription")]
    public string? ShortDescription { get; init; }

    [JsonProperty("longDesription")]
    public string? Description { get; init; }

    [JsonProperty("badge")]
    public string? Badge { get; init; }

    [JsonProperty("stats")]
    public BotStats Stats { get; init; }

    [JsonProperty("status")]
    public string Status { get; init; }
}