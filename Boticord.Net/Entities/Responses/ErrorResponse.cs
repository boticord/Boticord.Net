using Newtonsoft.Json;

namespace Boticord.Net.Entities;

public class ErrorResponse
{
    [JsonProperty("error")]
    public Error Error { get; init; }
}

public class Error
{
    [JsonProperty("code")]
    public string Code { get; init; }

    [JsonProperty("message")]
    public string Message { get; init; }
}