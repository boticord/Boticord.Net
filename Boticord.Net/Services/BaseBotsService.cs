using System;
using System.Collections.Generic;
using System.Formats.Asn1;
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

    private PeriodicTimer timer;

    public void AutoPostStats(TimeSpan interval, uint servers, uint shards = 1, uint users = 0, CancellationToken? cancellationToken = null)
    {
        timer = new(interval);

        if (interval < _client.BotsStatsRateLimit)
            throw new ArgumentOutOfRangeException(
                nameof(interval),
                interval,
                nameof(interval) + " value can not be less than " + _client.BotsStatsRateLimit);

        _ = Task.Run(async () =>
        {
            while (cancellationToken is null || !cancellationToken.Value.IsCancellationRequested)
            {
                await _client.SendBotStatsAsync(servers, shards, users);
                await timer.WaitForNextTickAsync(cancellationToken ?? default);
            }
        });
    }
}
