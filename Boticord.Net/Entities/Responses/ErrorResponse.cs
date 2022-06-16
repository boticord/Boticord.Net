using Newtonsoft.Json;

namespace Boticord.Net.Entities;

/// <summary>
/// Class containing an error returned from the API
/// </summary>
public class ErrorResponse
{
    [JsonProperty("error")]
    public Error Error { get; init; }
}

/// <summary>
/// Information about the error
/// </summary>
public class Error
{
    [JsonProperty("code")]
    public int Code { get; init; }

    [JsonProperty("message")]
    public string Message { get; init; }
}