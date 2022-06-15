![Boticord Logo](https://media.discordapp.net/attachments/985686409039970345/986336905429913650/logo.png "image")
<div align="center ">
    <p>
        <a href="https://discord.gg/hkHjW8a"><img src="https://img.shields.io/discord/722424773233213460?color=7289da&label=Discord&logo=discord&logoColor=white" alt="Online"></a>
          <a href="https://www.nuget.org/packages/Boticord.Net/">
    <img src="https://img.shields.io/nuget/vpre/Boticord.Net.svg?maxAge=2592000?style=plastic" alt="NuGet">
  </a>
    </p>
</div>

## Installation

**Package Manager**

```
Install-Package Boticord.Net -Version 1.0.3
```

```
dotnet add package Boticord.Net --version 1.0.3
```
* [Nuget](https://www.nuget.org/packages/Boticord.Net/)

## Useful Links

* [GitHub](https://github.com/boticord)
* [Documentation](https://alxelzot.gitbook.io/boticord.net/)
* [BotiCord](https://boticord.top/)

## Example usage
This example uses [Discord.Net](https://github.com/discord-net/Discord.Net) to run discord bot
```cs
using Discord.WebSocket;
using Discord;
using Boticord.Net;


var boticordClient = new BoticordClient(new BoticordConfig
{
    Token = "boticord token here"
});

var botClient = new DiscordSocketClient();
botClient.Ready += BotReady;
botClient.Log += BotLog;

await botClient.LoginAsync(TokenType.Bot, "bot token here");
await botClient.StartAsync();

await Task.Delay(-1);

async Task BotReady()
{
    Console.WriteLine($"Logged into {botClient.CurrentUser}");

    _ = Task.Run(() => AutoUpdateBotStatus());

}

async Task AutoUpdateBotStatus()
{
    var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

    while (true)
    {
        await boticordClient.SendBotStatsAsync((uint)botClient.Guilds.Count,
            users: (uint)botClient.Guilds.Select(x => x.MemberCount).Sum());
        await timer.WaitForNextTickAsync();
    }
}

async Task BotLog(LogMessage message)
{
    Console.WriteLine(message.Message);
}

```
