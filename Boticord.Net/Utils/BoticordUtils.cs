using Boticord.Net.Enums;

namespace Boticord.Net.Utils;

internal static class BoticordUtils
{
    internal static readonly Dictionary<Endpoints, TokenType[]> AccessMap = new()
    {
        { Endpoints.GetBotInfo, new [] { TokenType.Bot , TokenType.PrivateBot, TokenType.Profile, TokenType.None} },
        { Endpoints.GetBotComments, new [] { TokenType.Bot , TokenType.PrivateBot, TokenType.Profile} },
        { Endpoints.PostBotStats, new [] { TokenType.Bot } },

        { Endpoints.GetServerInfo, new [] { TokenType.Bot , TokenType.PrivateBot, TokenType.Profile, TokenType.None} },
        { Endpoints.GetServerComments, new [] { TokenType.Bot , TokenType.PrivateBot, TokenType.Profile} },
        { Endpoints.PostServerStats, new [] { TokenType.Bot, TokenType.PrivateBot } },
        
        { Endpoints.PostLinksGet, new [] { TokenType.Profile } },
        { Endpoints.PostLinksCreate, new [] { TokenType.Profile } },
        { Endpoints.PostLinksDelete, new [] { TokenType.Profile } },
    };

    internal static bool CanSendRequestToEndpoint(BoticordClient client, Endpoints endpoint)
    => AccessMap[endpoint].Contains(client.Config.TokenType);
}