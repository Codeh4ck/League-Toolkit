namespace LeagueToolkit.Core.Models;

public class AuthenticationInfo
{
    public string ClientAuthToken { get; set; }
    public int ClientPort { get; set; }
    public string RemoteAuthToken { get; set; }
    public int RemotePort { get; set; }
}