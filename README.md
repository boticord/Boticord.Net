<div align="center ">

<a href="https://boticord.top"><img src="https://media.discordapp.net/attachments/985682556718563408/985999985084620890/68747470733a2f2f6d65676f72752e72752f626f7469636f7264617069322e706e67.png?width=1439&height=402"  alt="boticord.js"/></a>

<p>
    <a href="https://discord.gg/hkHjW8a"><img src="https://img.shields.io/discord/722424773233213460?color=7289da&label=Discord&logo=discord&logoColor=white" alt="Online"></a>
</p>

<br>Coming soon...</br>
</div>


## Example usage
This example uses Discord.Net to run discord bot
```cs
using Discord.WebSocket;
using Discord;
using Boticord.Net;
using Boticord.Net.Services;

var botClient = new DiscordSocketClient();
botClient.Ready += BotReady;

var boticordClient = new BoticordClient(new BoticordConfig
{
    Token = "boticord token here"
});

await botClient.LoginAsync(TokenType.Bot, "bot token here");
await botClient.StartAsync();

await Task.Delay(-1);

async Task BotReady()
{
    Console.WriteLine($"Logged into {botClient.CurrentUser}");

    //run auto post things here
}

```