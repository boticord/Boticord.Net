using System.Net;
using System.Net.Http.Headers;
using System.Text;

using Newtonsoft.Json;

using Boticord.Net.Utils;
using Boticord.Net.Enums;

using Boticord.Net.Services;
using Boticord.Net.Core;


namespace Boticord.Net;


/// <summary>
/// Boticord API Rest client
/// </summary>
public class BoticordClient
{
    internal HttpClient HttpClient;

    internal const string BaseUrl = "https://api.boticord.top/v2/";

    /// <summary>
    /// The config client uses
    /// </summary>
    public BoticordConfig Config;
    
    /// <summary>
    /// REST client for Boticord API.
    /// </summary>
    /// <param name="config">Configuration object</param>
    /// <exception cref="InvalidDataException">The supplied token was invalid</exception>
    public BoticordClient(BoticordConfig config)
    {
        Config = config;
        HttpClient = config.HttpClient ?? new HttpClient();
        HttpClient.DefaultRequestHeaders.Add("Authorization", config.Token);

        if (!ValidateTokenAsync(Config.Token).GetAwaiter().GetResult())
            throw new InvalidDataException("The supplied token was invalid");
    }

    internal readonly TimeSpan BotsStatsRateLimit = TimeSpan.FromSeconds(2);

    private BucketRateLimiter _mainLimiter = new (5, TimeSpan.FromSeconds(6));
    private BucketRateLimiter _secondaryLimiter = new (1, TimeSpan.FromSeconds(3));

    private bool _rateLimitTriggered;

    private async Task RateLimit(bool postStats = false)
    {
        if (_rateLimitTriggered)
            await Task.Delay(6000);

        await _mainLimiter.WaitAsync();

        if (postStats)
            await _secondaryLimiter.WaitAsync();
    }

    private async Task<T> Request<T>(HttpRequestMessage request)
    {
        await RateLimit(request.RequestUri!.ToString().EndsWith("stats") || request.RequestUri!.ToString().EndsWith("server"));

        request.Headers.Authorization = AuthenticationHeaderValue.Parse(Config.Token);

        var response = await HttpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync())!;

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            _rateLimitTriggered = true;
            return await Request<T>(request);
        }

        try
        {
            var error = JsonConvert.DeserializeObject<ErrorResponse>(await response.Content.ReadAsStringAsync())!;
            throw new HttpRequestException(error.Error.Message, null, (HttpStatusCode)error.Error.Code);
        }
        catch (JsonException)
        {
            throw new HttpRequestException(await response.Content.ReadAsStringAsync(), null, response.StatusCode);
        }
    }

    internal async Task<T> PostRequest<T>(string path, StringContent data)
        => await Request<T>(new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}{path}") { Content = data });

    internal async Task<T> GetRequest<T>(string path, TimeSpan? timeout = null) 
        => await Request<T>(new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}{path}"));

    internal async Task<bool> ValidateTokenAsync(string constructedToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}token");
        request.Headers.Authorization = AuthenticationHeaderValue.Parse(constructedToken);
        var response = await HttpClient.SendAsync(request);

        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Checks if supplied token is valid.
    /// </summary>
    /// <param name="token">Token string</param>
    /// <param name="tokenType">Type of the token</param>
    /// <returns>If the supplied token is valid</returns>
    public Task<bool> ValidateTokenAsync(string token, TokenType tokenType)
        => ValidateTokenAsync($"{tokenType} {token}");

    private void ThrowIfNoAccess(Endpoints endpoint)
    {
        if (!BoticordUtils.CanSendRequestToEndpoint(this, endpoint))
            throw new InvalidOperationException($"Cannot access {endpoint} endpoint with token type {Config.TokenType}");
    }

    /// <summary>
    /// Gets information about a bot by its' id.
    /// </summary>
    /// <param name="botId">The id of the bot</param>
    /// <returns>Bot information</returns>
    /// <exception cref="HttpRequestException"></exception>
    public Task<BotInfo> GetBotInfoAsync(ulong botId)
    {
        ThrowIfNoAccess(Endpoints.GetBotInfo);

        return GetRequest<BotInfo>($"bot/{botId}");
    }

    /// <summary>
    /// Gets information about a bot by its' short code.
    /// </summary>
    /// <param name="shortId">The short id of the bot</param>
    /// <returns>Bot information</returns>
    /// <exception cref="HttpRequestException"></exception>
    public Task<BotInfo> GetBotInfoAsync(string shortId)
    {
        ThrowIfNoAccess(Endpoints.GetBotInfo);

        return GetRequest<BotInfo>($"bot/{shortId}");
    }

    /// <summary>
    /// Gets comments from bot's page.
    /// </summary>
    /// <remarks>Requires <see cref="TokenType"/> Bot, PrivateBot or Profile</remarks>
    /// <param name="botId">The id of the bot</param>
    /// <returns>Collection of <see cref="Comment"/>></returns>
    /// <exception cref="HttpRequestException"></exception>
    public Task<IEnumerable<Comment>> GetBotCommentsAsync(ulong botId)
    {
        ThrowIfNoAccess(Endpoints.GetBotComments);

        return GetRequest<IEnumerable<Comment>>($"bot/{botId}/comments");
    }

    /// <summary>
    /// Sends bot statistics.
    /// </summary>
    /// <remarks>Requires <see cref="TokenType"/> Bot</remarks>
    /// <param name="servers">Total server count of the bot</param>
    /// <param name="shards">Total shard count of the bot client</param>
    /// <param name="users">Total user count on all servers bot is in</param>
    /// <returns>Result of the operation</returns>
    /// <exception cref="HttpRequestException"></exception>
    public Task<OkResponse> SendBotStatsAsync(uint servers, uint shards = 1, uint users = 0)
    {
        ThrowIfNoAccess(Endpoints.PostBotStats);

        var content =
            new StringContent(JsonConvert.SerializeObject(new { servers, shards, users }), Encoding.UTF8, "application/json");
        return PostRequest<OkResponse>("stats", content);
    }

    /// <summary>
    /// Gets information about a server with supplied id.
    /// </summary>
    /// <param name="serverId">The id of the server</param>
    /// <returns>Information about user</returns>
    /// <exception cref="HttpRequestException"></exception>
    public Task<ServerInfo> GetServerInfoAsync(ulong serverId)
    {
        ThrowIfNoAccess(Endpoints.GetServerInfo);

        return GetRequest<ServerInfo>($"server/{serverId}");
    }

    /// <summary>
    /// Gets information about a server with supplied short id.
    /// </summary>
    /// <param name="shortId">The short id of the server</param>
    /// <returns>Information about user</returns>
    /// <exception cref="HttpRequestException"></exception>
    public Task<ServerInfo> GetServerInfoAsync(string shortId)
    {
        ThrowIfNoAccess(Endpoints.GetServerInfo);

        return GetRequest<ServerInfo>($"server/{shortId}");
    }

    /// <summary>
    /// Gets comments from server's page.
    /// </summary>
    /// <remarks>Requires <see cref="TokenType"/> Bot, PrivateBot or Profile</remarks>
    /// <param name="serverId">The id of the server</param>
    /// <returns>Collection of <see cref="Comment"/>></returns>
    /// <exception cref="HttpRequestException"></exception>
    public Task<IEnumerable<Comment>> GetServerCommentsAsync(ulong serverId)
    {
        ThrowIfNoAccess(Endpoints.GetServerComments);

        return GetRequest<IEnumerable<Comment>>($"server/{serverId}/comments");
    }

    /// <summary>
    /// Sends server statistics.
    /// </summary>
    /// <remarks>Requires <see cref="TokenType"/> Bot or PrivateBot</remarks>
    /// <param name="serverId">The id of the server</param>
    /// <param name="up">Try UP the server if its possible</param>
    /// <param name="status">The bot is member of the server</param>
    /// <param name="serverName">The name of the server</param>
    /// <param name="serverAvatar">Link to the avatar of the server</param>
    /// <param name="serverMembersAllCount">Total member count on the server</param>
    /// <param name="serverMembersOnlineCount">Online member count on the server</param>
    /// <param name="serverOwnerId">The id of the owner of the server</param>
    /// <returns>Result of the operation</returns>
    /// <exception cref="HttpRequestException"></exception>
    public Task<OkResponse> SendServerStatsAsync(ulong serverId, bool up, bool status,
        string? serverName = null,
        string? serverAvatar = null,
        uint? serverMembersAllCount = null,
        uint? serverMembersOnlineCount = null,
        ulong? serverOwnerId = null)
    {
        ThrowIfNoAccess(Endpoints.PostServerStats);

        var cont = new Dictionary<string, object>
        {
            { "serverID", serverId.ToString() },
            { "up", up ? 1 : 0 },
            { "status", status ? 1 : 0 },
        };
        if (serverName is not null)
            cont["serverName"] = serverName;

        if(serverAvatar is not null)
            cont["serverAvatar"] = serverAvatar;

        if (serverMembersAllCount is not null)
            cont["serverMembersAllCount"] = serverMembersAllCount;

        if (serverMembersOnlineCount is not null)
            cont["serverMembersOnlineCount"] = serverMembersOnlineCount;

        if (serverOwnerId is not null)
            cont["serverOwnerID"] = serverOwnerId.ToString()!;

        var content = new StringContent(JsonConvert.SerializeObject(cont), Encoding.UTF8, "application/json");
        return PostRequest<OkResponse>("server", content);
    }

    /// <summary>
    /// Gets all shortened links of the current user or all links with supplied code.
    /// </summary>
    /// <remarks>Requires <see cref="TokenType"/> Profile</remarks>
    /// <param name="code">The code to search links with</param>
    /// <returns>A collection of <see cref="ShortLink"/></returns>
    /// <exception cref="HttpRequestException"></exception>
    public Task<IEnumerable<ShortLink>> GetUsersShortLinksAsync(string? code = null)
    {
        ThrowIfNoAccess(Endpoints.PostLinksGet);
        var content = code is not null 
            ? new StringContent(JsonConvert.SerializeObject(new { code }), Encoding.UTF8, "application/json") 
            : new StringContent("{}", Encoding.UTF8, "application/json");

        return PostRequest<IEnumerable<ShortLink>>("links/get", content);
    }

    /// <summary>
    /// Creates shortened link with the supplied code.
    /// </summary>
    /// <remarks>To creates link that points to any domain other that boticord.top you need to purchase premium
    /// <br/>Requires <see cref="TokenType"/> Profile</remarks>
    /// <param name="code">The code of the shortened link</param>
    /// <param name="link">The link to shorten</param>
    /// <param name="domain">The domain for shortened link - bcord.cc, myservers.me or discord.camp</param>
    /// <returns>Result of the operation</returns>
    /// <exception cref="HttpRequestException"></exception>
    public Task<OkResponse> CreateShortLinksAsync(string code, string link, ShortDomain domain = ShortDomain.BcordCC)
    {
        ThrowIfNoAccess(Endpoints.PostLinksCreate);
        return PostRequest<OkResponse>("links/create",
            new StringContent(JsonConvert.SerializeObject(new { code, link, domain = (int)domain }), Encoding.UTF8, "application/json"));
    }

    /// <summary>
    /// Deletes shortened link with the supplied code.
    /// </summary>
    /// <remarks>Requires <see cref="TokenType"/> Profile</remarks>
    /// <param name="code">The code of the shortened link</param>
    /// <param name="domain">The domain for shortened link - bcord.cc, myservers.me or discord.camp</param>
    /// <returns>Result of the operation</returns>
    /// <exception cref="HttpRequestException"></exception>
    public Task<OkResponse> DeleteShortLinksAsync(string code, ShortDomain domain = ShortDomain.BcordCC)
    {
        ThrowIfNoAccess(Endpoints.PostLinksDelete);

        return PostRequest<OkResponse>("links/delete",
            new StringContent(JsonConvert.SerializeObject(new { code, domain = (int)domain }), Encoding.UTF8, "application/json"));
    }

    /// <summary>
    /// Gets information about a user with supplied id.
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <returns>Information about the user</returns>
    /// <exception cref="HttpRequestException"></exception>
    public Task<UserInfo> GetUserInfoAsync(ulong userId)
    {
        ThrowIfNoAccess(Endpoints.GetUserInfo);

        return GetRequest<UserInfo>($"profile/{userId}");
    }

    /// <summary>
    /// Gets comments of the user with the supplied id.
    /// </summary>
    /// <remarks>Requires <see cref="TokenType"/> Bot, PrivateBot or Profile</remarks>
    /// <param name="userId">The id of the user</param>
    /// <returns>An object, containing collections of <see cref="Comment"/>></returns>
    /// <exception cref="HttpRequestException"></exception>
    public Task<UserComments> GetUserCommentsAsync(ulong userId)
    {
        ThrowIfNoAccess(Endpoints.GetUserInfo);

        return GetRequest<UserComments>($"profile/{userId}/comments");
    }

    /// <summary>
    /// Gets bot's added but the user with supplied id.
    /// </summary>
    /// <remarks>Requires <see cref="TokenType"/> Bot, PrivateBot or Profile</remarks>
    /// <param name="userId">The id of the user</param>
    /// <returns>A collections of <see cref="UsersBot"/>></returns>
    /// <exception cref="HttpRequestException"></exception>
    public Task<IEnumerable<UsersBot>> GetUsersBotsAsync(ulong userId)
    {
        ThrowIfNoAccess(Endpoints.GetUserInfo);

        return GetRequest<IEnumerable<UsersBot>>($"bots/{userId}");
    }
}

