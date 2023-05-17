using Spectre.Console;
using Newtonsoft.Json;
using LeagueToolkit.Core;
using LeagueToolkit.SoloQueueReveal.Models;
using LeagueToolkit.SoloQueueReveal.Utilities;

namespace LeagueToolkit.SoloQueueReveal;

public delegate void OnLobbyUpdate(LobbyHandler handler, Dictionary<string, QueueMap> players);
public delegate void OnLobbyError(LobbyHandler handler, Exception ex);

public class LobbyHandler
{
    private readonly ILeagueApi _clientApi;
    private readonly ILeagueApi _remoteApi;

    private bool _isRunning;
    
    private Models.Region _region;

    private string[] _cache;
    private Dictionary<string, QueueMap> _cachedPlayerStats;

    public event OnLobbyUpdate OnLobbyUpdate;
    public event OnLobbyError OnLobbyError;


    public LobbyHandler(ILeagueApi clientApi, ILeagueApi remoteApi)
    {
        _clientApi = clientApi ?? throw new ArgumentNullException(nameof(clientApi));
        _remoteApi = remoteApi ?? throw new ArgumentNullException(nameof(remoteApi));

        _isRunning = false;
        
        _region = Models.Region.Unknown;
        
        _cache = Array.Empty<string>();
        _cachedPlayerStats = new();
    }

    public string[] GetTeamMemberNames() => _cache;
    public Dictionary<string, QueueMap> GetTeamMemberStats() => _cachedPlayerStats;
    public Models.Region GetRegion() => _region;

    public void Start(CancellationToken token = default)
    {
        _isRunning = true;
        Task.Factory.StartNew(() => Loop(token), token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    public void Stop() => _isRunning = false;

    private async Task Loop(CancellationToken token = default)
    {
        ConsoleUtility.WriteColorLine(Color.Blue, "Lobby handler started. Looking for lobby...", true);

        while (!token.IsCancellationRequested && _isRunning)
        {
            await Task.Delay(2000, token);

            try
            {
                UserInfoActual userInfoActual = null;

                if (_region == Models.Region.Unknown)
                {
                    ConsoleUtility.WriteColorLine(Color.Yellow, "Querying client API for user info...", true);

                    string userInfoResponse = await _clientApi.MakeRequestAsync(HttpMethod.Get, Endpoints.GetUserInfoEndpoint);
                    if (string.IsNullOrEmpty(userInfoResponse)) continue;

                    UserInfo userInfo = JsonConvert.DeserializeObject<UserInfo>(userInfoResponse);
                    if (userInfo == null) continue;

                    ConsoleUtility.WriteColorLine(Color.Green1, "Successfully retrieved user info.", true);

                    userInfoActual = JsonConvert.DeserializeObject<UserInfoActual>(userInfo.Info);
                    if (userInfoActual == null) continue;

                    Table table = new();

                    table.AddColumn("Property");
                    table.AddColumn("Value");

                    table.AddRow(new Markup("[blue]Summoner Name[/]"), new Markup($"[bold green]{userInfoActual.AccountData.SummonerName}[/]"));
                    table.AddRow(new Markup("[blue]Summoner Level[/]"), new Markup($"[bold green]{userInfoActual.AccountData.SummerLevel}[/]"));
                    AnsiConsole.Write(table);

                    ConsoleUtility.WriteColorLine(Color.Yellow, "Parsing region from user info...", true);

                    if (!Enum.TryParse(userInfoActual.LeagueOfLegends.Platform, true, out Platform platform))
                    {
                        ConsoleUtility.WriteColorLine(Color.Red,
                            "Could not parse region from League of Legends client data. Defaulting to Europe West (EUW).",
                            true);

                        _region = Models.Region.Euw;
                    }
                    else
                    {
                        _region = (Models.Region)platform;
                        ConsoleUtility.WriteColorLine(Color.Green1, $"Successfully parsed region: {_region.ToString("F").ToUpper()}.", true);
                    }
                }

                if (userInfoActual != null)
                {
                    ConsoleUtility.WriteColorLine(Color.Yellow,
                        $"Querying remote API for ranked statistics of {userInfoActual.AccountData.SummonerName}...",
                        true);
                    
                    QueueMap playerRankedStats =
                        await LeagueUtilities.GetCurrentSummonerRankedStats(_remoteApi, userInfoActual);

                    if (playerRankedStats != null)
                    {
                        ConsoleUtility.WriteColorLine(Color.Green1,
                            $"Successfully fetched ranked statistics of {userInfoActual.AccountData.SummonerName}",
                            true);

                        Table table = new();
                        table.AddColumn("Queue Type");
                        table.AddColumn("Tier");
                        table.AddColumn("Division");
                        table.AddColumn("LP");
                        table.AddColumn("Is Provisional");

                        table.AddRow(
                            new Markup($"[blue]Solo Queue[/]"),
                            new Markup($"[bold green]{playerRankedStats.SoloQueue.Tier}[/]"),
                            new Markup($"[bold green]{playerRankedStats.SoloQueue.Division}[/]"),
                            new Markup($"[bold green]{playerRankedStats.SoloQueue.LeaguePoints}[/]"),
                            new Markup($"[bold green]{playerRankedStats.SoloQueue.IsProvisional}[/]"));

                        table.AddRow(
                            new Markup($"[blue]Flex Queue[/]"),
                            new Markup($"[bold green]{playerRankedStats.FlexQueue.Tier}[/]"),
                            new Markup($"[bold green]{playerRankedStats.FlexQueue.Division}[/]"),
                            new Markup($"[bold green]{playerRankedStats.FlexQueue.LeaguePoints}[/]"),
                            new Markup($"[bold green]{playerRankedStats.FlexQueue.IsProvisional}[/]"));

                        AnsiConsole.Write(table);
                    }
                }

                string lobbyParticipantsResponse = await _clientApi.MakeRequestAsync(HttpMethod.Get, Endpoints.GetLobbyParticipantsEndpoint);
                if (string.IsNullOrEmpty(lobbyParticipantsResponse)) continue;

                Participants participants = JsonConvert.DeserializeObject<Participants>(lobbyParticipantsResponse);

                if (participants is not { TeamMembers: not null } || participants.TeamMembers.Count == 0)
                {
                    ConsoleUtility.WriteColorLine(Color.Red1, $"Could not find lobby participants. Are you in champ select?", true);
                    continue;
                }

                string[] playerNames = participants.TeamMembers.Select(x => x.Name).ToArray();

                if (!_cache.SequenceEqual(playerNames))
                {
                    Dictionary<string, QueueMap> playersWithStats = new();
                    
                    foreach (string playerName in playerNames)
                    {
                        QueueMap playerRankedStats = await LeagueUtilities.GetSummonerRankedStats(_remoteApi, playerName);
                        playersWithStats.Add(playerName, playerRankedStats);
                    }
                    
                    _cache = playerNames;
                    _cachedPlayerStats = playersWithStats;

                    OnLobbyUpdate?.Invoke(this, playersWithStats);
                }
            }
            catch (Exception ex)
            {
                OnLobbyError?.Invoke(this, ex);
                break;
            }
        }
    }
}