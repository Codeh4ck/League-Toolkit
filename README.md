# League Toolkit

League Toolkit is a heavily modified version of the [0xInception - Ekko](https://github.com/0xInception/Ekko) library and the [Riotphobia - Lobby Reveal](https://github.com/Riotphobia/LobbyReveal) tool.

**LeagueToolkit.Core** allows interaction with the LeagueClientUx API, the API that Riot uses to communicate with their servers through the League of Legends client. The **LeagueToolkit.Core** library differs from **Ekko** in that it allows you to communicate with API that uses the remote API.

*For example, you can query for a summoner's profile data through **LeagueToolkit.Core***

**LeagueToolkit.SoloQueueReveal** reveals your teammates' names in a solo queue game. It differs from **Lobby Reveal** in its data presentation and has been improved with some QoL changes. In addition to revealing your lobby, their ranks (tier, division and LP) will be displayed immediately, alongside a URL pointing to the specific player's op.gg profile.

## Installation & usage

No installation is needed to run **Solo Queue Reveal**. Make sure you have the prerequisites installed and follow the usage instructions outlined below.

### Prerequisites

* .NET 6 Runtime (Download the runtime from [Microsoft](https://dotnet.microsoft.com/en-us/download/dotnet/6.0))

### Usage

* Download the latest stable release from [releases](https://github.com/Codeh4ck/League-Toolkit/releases/tag/v1.0.0)
* Double click **LeagueToolkit.SoloQueueReveal.exe**
* Wait for it to detect your League of Legends client if you've opened it before the client
* Once you're in a solo queue lobby, it will attempt to grab the participant names and display them to you# League Toolkit

### TODO

*Fetch more info about lobby participants (winrate, losers queue, etc.)
*Differentiate data presented when in Flex or Normal queue
*Make some predictions regarding the game outcome based on player performance
