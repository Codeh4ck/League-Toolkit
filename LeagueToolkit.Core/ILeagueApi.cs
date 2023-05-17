namespace LeagueToolkit.Core;

public interface ILeagueApi
{
    Task<string> MakeRequestAsync(HttpMethod method, string endpoint, HttpContent httpContent = null);
}