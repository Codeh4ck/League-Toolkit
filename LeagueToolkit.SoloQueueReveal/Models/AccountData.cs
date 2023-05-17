using Newtonsoft.Json;

namespace LeagueToolkit.SoloQueueReveal.Models;

public class AccountData
{
    [JsonProperty(PropertyName = "summoner_name")]
    public string SummonerName { get; set; }
    
    [JsonProperty(PropertyName = "summoner_level")]
    public int SummerLevel { get; set; }
}