using Spectre.Console;
using System.Web;
using System.Diagnostics;
using System.Reflection;
using LeagueToolkit.Core;
using LeagueToolkit.SoloQueueReveal.Models;
using LeagueToolkit.SoloQueueReveal.Utilities;

namespace LeagueToolkit.SoloQueueReveal;

public class Program
{
    private static bool _update = false;
    private static CancellationTokenSource _cts;
    private static Dictionary<Guid, LobbyHandler> _lobbyHandlers;

    private const string MutexIdentifier = "LeagueToolkitSingleInstanceConsoleSQR";
    
    public static async Task Main(string[] args)
    {
        using Mutex mutex = new(false, MutexIdentifier);
        
        if (!mutex.WaitOne(0))
        {
            ConsoleUtility.WriteColorLine(Color.Red, "League Toolkit - Solo Queue Reveal is already running.", true);
            await Task.Delay(TimeSpan.FromSeconds(5));
            Environment.Exit(0);
        }
            
        _cts = new();
        _lobbyHandlers = new();

        Guid titleGuid = Guid.NewGuid();
        Console.Title = $"Instance: {titleGuid:D}";
        
        FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
        string version = fileVersionInfo.ProductVersion;

        ConsoleUtility.WriteColorLine(Color.Blue, $"League Toolkit - Solo Queue Reveal - Version: {version}", true);
        ConsoleUtility.WriteColorLine(Color.Blue,"GitHub: https://github.com/Codeh4ck/League-Toolkit", true);

        ConsoleUtility.WriteColorLine(Color.Yellow, "Starting league client observer. Waiting for a client to open...", true);
        LeagueClientObserver observer = new();
        
        observer.ClientOpened += (sender, client) =>
        {
            ConsoleUtility.WriteColorLine(Color.Blue,$"A League of Legends client has been found. Client ID: {client.ClientId}", true);

            LobbyHandler handler = new 
            (
                new LeagueClientApi(client.AuthenticationInfo.ClientAuthToken, client.AuthenticationInfo.ClientPort), 
                new LeagueRemoteApi(client.AuthenticationInfo.RemoteAuthToken, client.AuthenticationInfo.RemotePort)
            );
            
            _lobbyHandlers.Add(client.ClientId, handler);
            
            handler.OnLobbyUpdate += (_, _) => _update = true;
            
            handler.OnLobbyError += (_, exception) =>
            {
                ConsoleUtility.WriteColorLine(Color.Red,$"An exception was thrown: {exception.Message}");
            };
            
            handler.Start(_cts.Token);
        };

        observer.ClientClosed += (_, client) =>
        {
            _lobbyHandlers[client.ClientId].Stop();
            _lobbyHandlers.Remove(client.ClientId);
            ConsoleUtility.WriteColorLine(Color.Red,$"A League of Legends client has been closed. Client ID: {client.ClientId}", true);
        };

#pragma warning disable CS4014
        Task.Factory.StartNew(async () => await observer.Observe(_cts.Token), _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        Task.Factory.StartNew(async () => await Refresh(_cts.Token), _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
#pragma warning restore CS4014

        while (true)
        {
            ConsoleKeyInfo info = Console.ReadKey();
            if (info.Key != ConsoleKey.Escape) continue;
            
            Environment.Exit(0);
            return;
        }
    }
    
    private static async Task Refresh(CancellationToken token = default)
    {
        while (!token.IsCancellationRequested)
        {
            if (_update)
            {
                ConsoleUtility.WriteColorLine(Color.Green1, "Found lobby participants:", true);
                
                foreach (KeyValuePair<Guid, LobbyHandler> lobbyHandlerPair in _lobbyHandlers)
                {
                    Table table = new();
                    table.AddColumn("Client ID");
                    table.AddColumn("Player Name");
                    table.AddColumn("Tier");
                    table.AddColumn("Division");
                    table.AddColumn("LP");
                    table.AddColumn("Op.gg URL");

                    Dictionary<string, QueueMap> playerNamesWithStats = lobbyHandlerPair.Value.GetTeamMemberStats();
                    
                    foreach (string playerName in lobbyHandlerPair.Value.GetTeamMemberNames())
                    {
                        string opggUrl = $"https://op.gg/summoners/{lobbyHandlerPair.Value.GetRegion().ToString("F").ToLower()}/{HttpUtility.UrlEncode(playerName)}";

                        string tier = "N/A";
                        string division = "N/A";
                        string lp = "N/A";
                        
                        bool success = playerNamesWithStats.TryGetValue(playerName, out QueueMap playerStats);
                        
                        if (success)
                        {
                            tier = playerStats.SoloQueue.Tier;
                            division = playerStats.SoloQueue.Division;
                            lp = playerStats.SoloQueue.LeaguePoints.ToString();
                        }

                        table.AddRow(
                            new Markup($"[bold blue]{lobbyHandlerPair.Key}[/]"),
                            new Markup($"[bold blue]{playerName}[/]"),
                            new Markup($"[bold blue]{tier}[/]"),
                            new Markup($"[bold blue]{division}[/]"),
                            new Markup($"[bold blue]{lp}[/]"),
                            new Markup($"[bold blue]{opggUrl}[/]"));
                    }
                    
                    AnsiConsole.Write(table);
                    Console.WriteLine();
                }
                _update = false;
            }

            await Task.Delay(2000, token);
        }
    }
}