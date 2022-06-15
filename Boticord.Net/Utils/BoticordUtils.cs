using Boticord.Net.Enums;

namespace Boticord.Net.Utils;

internal static class BoticordUtils
{
    internal static readonly Dictionary<Endpoints, TokenType[]> AccessMap = new()
    {
        { Endpoints.GetBotInfo, new [] { TokenType.Bot , TokenType.PrivateBot, TokenType.Profile} },
        { Endpoints.GetBotComments, new [] { TokenType.Bot , TokenType.PrivateBot, TokenType.Profile} },
        { Endpoints.PostBotStats, new [] { TokenType.Bot } },
    };

    internal static bool CanSendRequestToEndpoint(BoticordClient client, Endpoints endpoint)
    => AccessMap[endpoint].Contains(client.Config.TokenType);
}