using LeagueToolkit.Core.Models;

namespace LeagueToolkit.Core;

public interface ILeagueClientObserver
{
    delegate void OnClientStatusChanged(LeagueClientObserver sender, LeagueClient client);

    public event OnClientStatusChanged ClientOpened;
    public event OnClientStatusChanged ClientClosed;
    Task Observe(CancellationToken token = default);
}