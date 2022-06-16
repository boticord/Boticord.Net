using System;
using System.Net.Http;

using Boticord.Net.Enums;

namespace Boticord.Net;


/// <summary>
/// A configuration class for <see cref="BoticordClient"/>.
/// </summary>
public class BoticordConfig
{
    private string _token = "";

    /// <summary>
    /// Initialize new object
    /// </summary>
    /// <param name="httpClient">Pass existing HTTP client instance or the library will create it's own</param>
    public BoticordConfig(HttpClient? httpClient = null)
    {
        HttpClient = httpClient;
    }

    /// <summary>
    /// Gets or sets <see cref="System.Net.Http.HttpClient"/> library uses. Set to existing instance or the library will create it's own
    /// </summary>
    public HttpClient? HttpClient { get; set; }

    /// <summary>
    /// Gets or set's the type of token the client will use. <see cref="Enums.TokenType.Bot"/> is the default value.
    /// </summary>
    public TokenType TokenType { get; set; } = TokenType.Bot;

    /// <summary>
    /// Gets or sets the token the client will use. Make sure type of token matches <see cref="TokenType"/>
    /// </summary>
    /// <exception cref="NullReferenceException">Token can not be null or empty</exception>
    public string Token
    {
        get => _token;
        set
        {
            if (TokenType != TokenType.None && string.IsNullOrWhiteSpace(value))
                throw new NullReferenceException("Token can not be null or empty");

            _token = $"{TokenType} {value}";
        }
    }
}
