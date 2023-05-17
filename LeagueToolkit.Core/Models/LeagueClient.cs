using System.Diagnostics;

namespace LeagueToolkit.Core.Models;

public class LeagueClient
{
    public Guid ClientId { get; set; }
    public string FilePath { get; set; }
    public Process Process { get; set; }
    public AuthenticationInfo AuthenticationInfo { get; set; }
}