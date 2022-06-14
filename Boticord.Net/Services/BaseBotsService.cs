using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Boticord.Net.Types;
using Boricord.Net.Types.Bots;
using Boticord.Net.Types.Interfaces;

namespace Boticord.Net.Services
{
    public class BaseBotsService : BaseService
    {
        public BaseBotsService(Boticord.NetClient client) : base(client) { }

        public void AutoPostStats(TimeSpan interval, ulong botId, uint shards = 1, uint servers = 1)
        {
            if (interval < Client.BotsRateLimit)
                throw new ArgumentOutOfRangeException(
                    nameof(interval),
                    interval, 
                    nameof(interval) + " value can not be less than" + Client.BotsRateLimit);

            _ = Task.Run(async () =>
            {
                while (true)
                {
                    await PostStats<BaseStatsResponse>(botId, shards, servers);
                    await Task.Delay(interval);
                }
            });
        }

        public async Task<T> PostStats<T>(ulong botId, uint shards = 1, uint servers = 1)
            where T : IStatsResponse
        {
            shards = shards < 1 ? 1 : shards;
            servers = servers < 1 ? 1 : servers;
            return await Client.PostRequest<T>(
                "/bots" + botId + "/stats",
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
                Client.BotsRateLimit
            );
        }
    }
}