using Newtonsoft.Json;

namespace Boticord.Net.Entities;

public class UpdateServerResponse
{
    [JsonProperty("serverID")]
    protected string _id { get; init; }

    [JsonIgnore]
    public ulong ServerId => ulong.Parse(_id);

    [JsonProperty("up")]
    public int Up { get; init; }

    [JsonProperty("updated")]
    public bool Updated { get; init; }

    [JsonProperty("bumps")]
    public int Bumps { get; init; }

    [JsonProperty("boost")]
    public bool Boost { get; init; }

    [JsonProperty("upSuccessfully")]
    public bool UpSuccessfully { get; init; }

    [JsonProperty("timeToNextUpInMs")]
    protected long _timeToNextUpInMs { get; init; }

    [JsonIgnore]
    public TimeSpan TimeToNextUpIn => TimeSpan.FromMilliseconds(_timeToNextUpInMs);

    [JsonProperty("message")]
    public string Message { get; init; }
}