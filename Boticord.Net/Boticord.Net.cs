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

namespace Boticord.Net;

public class BoticordClient
{
    internal HttpClient HttpClient;

    internal const string BaseUrl = "https://api.boticord.top/v2/";

    internal readonly TimeSpan RateLimit = TimeSpan.FromSeconds(2);
    internal readonly TimeSpan BotsRateLimit = TimeSpan.FromSeconds(30);
    private DateTime _lastRequest = DateTime.UtcNow;

    public BoticordConfig Config;

    public BoticordClient(BoticordConfig config)
    {
        Config = config;

        HttpClient = config.HttpClient ?? new HttpClient();

        HttpClient.DefaultRequestHeaders.Add("Authorization", config.Token);
    }

    private async Task RateLimiter(TimeSpan timeout)
    {
        while (DateTime.UtcNow - _lastRequest <= timeout)
            await Task.Delay(timeout - (DateTime.UtcNow - _lastRequest));

        _lastRequest = DateTime.UtcNow;
    }

    private async Task<T> Request<T>(HttpRequestMessage request, TimeSpan? timeout = null)
    {
        await RateLimiter(timeout ?? RateLimit);

        request.Headers.Authorization = AuthenticationHeaderValue.Parse(Config.Token);

        var response = await HttpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        if (response.IsSuccessStatusCode)
            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync())!;

        try
        {
            var error = JsonConvert.DeserializeObject<ErrorResponse>(await response.Content.ReadAsStringAsync())!;
            throw new HttpRequestException(error.Error.Message, null, (HttpStatusCode)error.Error.Code);
        }
        catch
        {
            throw new HttpRequestException(await response.Content.ReadAsStringAsync(), null, response.StatusCode);
        }
    }

    internal async Task<T> PostRequest<T>(string path, StringContent data, TimeSpan? timeout = null) =>

        await Request<T>(new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}{path}"){Content = data}, timeout);

    internal async Task<T> GetRequest<T>(string path, TimeSpan? timeout = null) =>
        await Request<T>(new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}{path}"), timeout);

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
        
        var content =
            new StringContent(JsonConvert.SerializeObject(new
            {
                serverId, 
                up = up ? 1 : 0, 
                status = status ? 1 : 0,
                serverName,
                serverAvatar,
                serverMembersAllCount,
                serverMembersOnlineCount,
                serverOwnerID
            }), Encoding.UTF8, "application/json");
        return PostRequest<OkResponse>("stats", content);
    }

    public Task<IEnumerable<ShortLink>> GetUsersShortLinks(string? code = null)
    {
        ThrowIfNoAccess(Endpoints.PostLinksGet);

        return PostRequest<IEnumerable<ShortLink>>("links/get",
            new StringContent(JsonConvert.SerializeObject(new { code })));
    }

    public Task<OkResponse> CreateShortLinks(string code, string link, ShortDomain domain = ShortDomain.BcordCC)
    {
        ThrowIfNoAccess(Endpoints.PostLinksCreate);

        return PostRequest<OkResponse>("links/create",
            new StringContent(JsonConvert.SerializeObject(new { code, link, domain = (int)domain })));
    }

    public Task<OkResponse> DeleteShortLinks(string code, ShortDomain domain = ShortDomain.BcordCC)
    {
        ThrowIfNoAccess(Endpoints.PostLinksDelete);

        return PostRequest<OkResponse>("links/delete",
            new StringContent(JsonConvert.SerializeObject(new { code, domain = (int)domain })));
    }
}

