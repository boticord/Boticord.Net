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
    internal HttpClient HttpClient = new()
    {
        BaseAddress = new Uri("https://api.boticord.top/v2/")
    };

    internal readonly TimeSpan RateLimit = TimeSpan.FromSeconds(2);
    internal readonly TimeSpan BotsRateLimit = TimeSpan.FromSeconds(30);
    private DateTime _lastRequest = DateTime.UtcNow;

    public BoticordConfig Config;

    public BoticordClient(BoticordConfig config)
    {
        Config = config;

        HttpClient = config.HttpClient ?? HttpClient;

        HttpClient.DefaultRequestHeaders.Add("Authorization", config.Token);
    }

    private async Task RateLimiter(TimeSpan timeout)
    {
        while (DateTime.UtcNow - _lastRequest <= timeout)
            await Task.Delay(timeout - (DateTime.UtcNow - _lastRequest));

        _lastRequest = DateTime.UtcNow;
    }

    private async Task<T> Request<T>(Task<HttpResponseMessage> task, TimeSpan? timeout = null)
    {
        await RateLimiter(timeout ?? RateLimit);

        var response = await task;
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
        await Request<T>(HttpClient.PostAsync(path, data), timeout);

    internal async Task<T> GetRequest<T>(string path, TimeSpan? timeout = null) =>
        await Request<T>(HttpClient.GetAsync(path), timeout);

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
}

