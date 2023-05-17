using System.Diagnostics;
using LeagueToolkit.Core.Models;
using LeagueToolkit.Core.OperatingSystems;

namespace LeagueToolkit.Core;

public class LeagueClientObserver : ILeagueClientObserver
{
    public event ILeagueClientObserver.OnClientStatusChanged ClientOpened;
    public event ILeagueClientObserver.OnClientStatusChanged ClientClosed;

    private readonly List<LeagueClient> _leagueClients;
    private readonly OperatingSystemBase _osBase;
    
    
    public LeagueClientObserver()
    {
        _leagueClients = new();
        _osBase = OperatingSystemFactory.GetOperatingSystem();
    }
    
    public async Task Observe(CancellationToken token = default)
    {
        while (!token.IsCancellationRequested)
        {
            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes.Where(p => p.ProcessName == Constants.LeagueClientProcessName || p.ProcessName == Constants.LeagueClientWineProcessName))
            {
                if (_leagueClients.Any(x => x.Process.Id == process.Id)) continue;

                AuthenticationInfo authInfo = _osBase.ExtractAuthenticationInfo(process.Id);
                if (authInfo == null) continue;

                LeagueClient client = new()
                {
                    ClientId = Guid.NewGuid(),
                    Process = process,
                    AuthenticationInfo = authInfo,
                    FilePath = process.MainModule?.FileName ?? string.Empty
                };
                
                process.EnableRaisingEvents = true;
                process.Exited += (sender, args) =>
                {
                    _leagueClients.Remove(client);
                    OnClientClosed(client);
                };
                
                _leagueClients.Add(client);
                OnClientOpened(client);
            }

            await Task.Delay(1000, token);
        }
    }
    
    public void OnClientOpened(LeagueClient client) => ClientOpened?.Invoke(this, client);
    public void OnClientClosed(LeagueClient client) => ClientClosed?.Invoke(this, client);
}