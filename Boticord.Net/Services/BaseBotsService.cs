using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Boticord.Net;
using Boticord.Net.Entities;

namespace Boticord.Net.Services;

public class BaseBotsService
{
    private readonly BoticordClient _client;

    public BaseBotsService(BoticordClient client)
    {
        _client = client;
    }

    public void AutoPostStats(TimeSpan interval, ulong botId, uint shards = 1, uint servers = 1)
    {
        if (interval < _client.BotsRateLimit)
            throw new ArgumentOutOfRangeException(
                nameof(interval),
                interval,
                nameof(interval) + " value can not be less than" + _client.BotsRateLimit);

        _ = Task.Run(async () =>
        {
            while (true)
            {
                await PostStats<UpdateServerStatsResponse>(botId, shards, servers);
                await Task.Delay(interval);
            }
        });
    }

    public async Task<T> PostStats<T>(ulong botId, uint shards = 1, uint servers = 1)
        where T : UpdateServerStatsResponse
    {
        shards = shards < 1 ? 1 : shards;
        servers = servers < 1 ? 1 : servers;
        return await _client.PostRequest<T>(
            "bots" + botId + "/stats",
            new StringContent(
                JsonConvert.SerializeObject(
                    new Dictionary<string, uint>
                    {
                            { "shardsCount", shards },
                            { "serversCount", servers }
                    }
                ),
                Encoding.ASCII,
                "application/json"
            ),
            _client.BotsRateLimit
        );
    }
}
