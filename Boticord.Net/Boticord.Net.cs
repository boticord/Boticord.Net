using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Boticord.Net.Entities;
using Boticord.Net.Utils;
using Boticord.Net.Enums;

using RateLimiter;
using ComposableAsync;

namespace Boticord.Net;

public class BoticordClient
{
    internal HttpClient HttpClient;

    internal const string BaseUrl = "https://api.boticord.top/v2/";

    public BoticordConfig Config;

    public BoticordClient(BoticordConfig config)
    {
        Config = config;
        HttpClient = config.HttpClient ?? new HttpClient();
        HttpClient.DefaultRequestHeaders.Add("Authorization", config.Token);
    }

    internal readonly TimeSpan BotsStatsRateLimit = TimeSpan.FromSeconds(2);

    private TimeLimiter _mainLimiter = TimeLimiter.GetFromMaxCountByInterval(5, TimeSpan.FromSeconds(6));
    private TimeLimiter _secondaryLimiter = TimeLimiter.GetFromMaxCountByInterval(1, TimeSpan.FromSeconds(3));

    private bool _rateLimitTriggered = false;

    private async Task RateLimiter(bool postStats = false)
    {
        if (_rateLimitTriggered)
            await Task.Delay(6000);
        await _mainLimiter;
        if (postStats)
            await _secondaryLimiter;
    }

    private async Task<T> Request<T>(HttpRequestMessage request)
    {
        await RateLimiter(request.RequestUri!.ToString().EndsWith("stats") || request.RequestUri!.ToString().EndsWith("server"));

        request.Headers.Authorization = AuthenticationHeaderValue.Parse(Config.Token);

        var response = await HttpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync())!;

        try
        {
            var error = JsonConvert.DeserializeObject<ErrorResponse>(await response.Content.ReadAsStringAsync())!;
            throw new HttpRequestException(error.Error.Message, null, (HttpStatusCode)error.Error.Code);
        }
        catch
        {
            if (response.StatusCode != HttpStatusCode.TooManyRequests)
                throw new HttpRequestException(await response.Content.ReadAsStringAsync(), null, response.StatusCode);

            _rateLimitTriggered = true;
            return await Request<T>(request);
        }
    }

    internal async Task<T> PostRequest<T>(string path, StringContent data) =>

        await Request<T>(new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}{path}") { Content = data });

    internal async Task<T> GetRequest<T>(string path, TimeSpan? timeout = null) =>
        await Request<T>(new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}{path}"));

    internal async Task<bool> ValidateToken(string token, TokenType tokenType)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}{token}");
        request.Headers.Authorization = AuthenticationHeaderValue.Parse($"{tokenType} {token}");

        var response = await HttpClient.SendAsync(request);

        return response.IsSuccessStatusCode;
    }

    private void ThrowIfNoAccess(Endpoints endpoint)
    {
        if (!BoticordUtils.CanSendRequestToEndpoint(this, endpoint))
            throw new InvalidOperationException($"Cannot access {endpoint} endpoint with token type {Config.TokenType}");
    }

    public Task<BotInfo> GetBotInfoAsync(ulong botId)
    {
        ThrowIfNoAccess(Endpoints.GetBotInfo);

        return GetRequest<BotInfo>($"bot/{botId}");
    }

    public Task<BotInfo> GetBotInfoAsync(string shortId)
    {
        ThrowIfNoAccess(Endpoints.GetBotInfo);

        return GetRequest<BotInfo>($"bot/{shortId}");
    }

    public Task<IEnumerable<Comment>> GetBotCommentsAsync(ulong botId)
    {
        ThrowIfNoAccess(Endpoints.GetBotComments);

        return GetRequest<IEnumerable<Comment>>($"bot/{botId}/comments");
    }

    public Task<OkResponse> SendBotStatsAsync(uint servers, uint shards = 1, uint users = 0)
    {
        ThrowIfNoAccess(Endpoints.PostBotStats);

        var content =
            new StringContent(JsonConvert.SerializeObject(new { servers, shards, users }), Encoding.UTF8, "application/json");
        return PostRequest<OkResponse>("stats", content);
    }

    public Task<ServerInfo> GetServerInfoAsync(ulong serverId)
    {
        ThrowIfNoAccess(Endpoints.GetServerInfo);

        return GetRequest<ServerInfo>($"server/{serverId}");
    }

    public Task<ServerInfo> GetServerInfoAsync(string shortId)
    {
        ThrowIfNoAccess(Endpoints.GetServerInfo);

        return GetRequest<ServerInfo>($"server/{shortId}");
    }

    public Task<IEnumerable<Comment>> GetServerCommentsAsync(ulong serverId)
    {
        ThrowIfNoAccess(Endpoints.GetServerComments);

        return GetRequest<IEnumerable<Comment>>($"server/{serverId}/comments");
    }

    public Task<OkResponse> SendServerStatsAsync(ulong serverId, bool up, bool status,
        string? serverName = null,
        string? serverAvatar = null,
        uint? serverMembersAllCount = null,
        uint? serverMembersOnlineCount = null,
        ulong? serverOwnerID = null)
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

        if (serverOwnerID is not null)
            cont["serverOwnerID"] = serverOwnerID.ToString()!;

        var content = new StringContent(JsonConvert.SerializeObject(cont), Encoding.UTF8, "application/json");
        return PostRequest<OkResponse>("server", content);
    }

    public Task<IEnumerable<ShortLink>> GetUsersShortLinksAsync(string? code = null)
    {
        ThrowIfNoAccess(Endpoints.PostLinksGet);
        var content = code is not null 
            ? new StringContent(JsonConvert.SerializeObject(new { code }), Encoding.UTF8, "application/json") 
            : new StringContent("{}", Encoding.UTF8, "application/json");

        return PostRequest<IEnumerable<ShortLink>>("links/get", content);
    }

    public Task<OkResponse> CreateShortLinksAsync(string code, string link, ShortDomain domain = ShortDomain.BcordCC)
    {
        ThrowIfNoAccess(Endpoints.PostLinksCreate);
        return PostRequest<OkResponse>("links/create",
            new StringContent(JsonConvert.SerializeObject(new { code, link, domain = (int)domain }), Encoding.UTF8, "application/json"));
    }

    public Task<OkResponse> DeleteShortLinksAsync(string code, ShortDomain domain = ShortDomain.BcordCC)
    {
        ThrowIfNoAccess(Endpoints.PostLinksDelete);

        return PostRequest<OkResponse>("links/delete",
            new StringContent(JsonConvert.SerializeObject(new { code, domain = (int)domain }), Encoding.UTF8, "application/json"));
    }

    public Task<UserInfo> GetUserInfoAsync(ulong userId)
    {
        ThrowIfNoAccess(Endpoints.GetUserInfo);

        return GetRequest<UserInfo>($"profile/{userId}");
    }

    public Task<UserComments> GetUserCommentsAsync(ulong userId)
    {
        ThrowIfNoAccess(Endpoints.GetUserInfo);

        return GetRequest<UserComments>($"profile/{userId}/comments");
    }

    public Task<IEnumerable<UsersBot>> GetUsersBotsAsync(ulong userId)
    {
        ThrowIfNoAccess(Endpoints.GetUserInfo);

        return GetRequest<IEnumerable<UsersBot>>($"bots/{userId}");
    }
}

