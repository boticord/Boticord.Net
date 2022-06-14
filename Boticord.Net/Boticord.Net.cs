using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Boticord.Net.Entities;

namespace Boticord.Net;

public class BoticordClient
{
    internal HttpClient HttpClient = new()
    {
        BaseAddress = new Uri("https://api.boticord.top/v1/"),
        DefaultRequestHeaders =
            {
                Accept = { new MediaTypeWithQualityHeaderValue("application/json") }
            }
    };

    internal readonly TimeSpan RateLimit = TimeSpan.FromSeconds(2);
    internal readonly TimeSpan BotsRateLimit = TimeSpan.FromSeconds(30);
    private DateTime _lastRequest;

    public BoticordClient(BoticordConfig config)
    {
        // Wrapper = config.Wrapper;
        HttpClient = config.HttpClient ?? HttpClient;

        HttpClient.DefaultRequestHeaders.Add("Authorization", config.Token);
    }

    private async Task RateLimiter(TimeSpan timeout)
    {
        while (DateTime.UtcNow - _lastRequest <= timeout || _lastRequest != new DateTime())
            await Task.Delay(timeout - (DateTime.UtcNow - _lastRequest));

        _lastRequest = DateTime.UtcNow;
    }

    private async Task<T> Request<T>(Task<HttpResponseMessage> task, TimeSpan? timeout = null)
    {
        await RateLimiter(timeout ?? RateLimit);

        var response = await task;
        response.EnsureSuccessStatusCode();

        if(response.IsSuccessStatusCode)
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


    public Task<BotInfo> GetBotInfoAsync(ulong botId)
    {
        return GetRequest<BotInfo>($"bot/{botId}");
    }

    public Task<BotInfo> GetBotInfoAsync(string shortId)
    {
        return GetRequest<BotInfo>($"bot/{shortId}");
    }

    public Task<IEnumerable<Comment>> GetBotCommentsAsync(ulong botId)
    {
        return GetRequest<IEnumerable<Comment>>($"bot/{botId}/comments");
    }

    public Task<IEnumerable<Comment>> GetBotCommentsAsync(string shortId)
    {
        return GetRequest<IEnumerable<Comment>>($"bot/{shortId}/comments");
    }
}

