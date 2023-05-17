using Newtonsoft.Json;

namespace LeagueToolkit.SoloQueueReveal.Models;

public class UserInfoActual
{
    [JsonProperty(PropertyName = "lol")]
    public LeagueOfLegends LeagueOfLegends { get; set; }
    
    [JsonProperty(PropertyName = "lol_account")]
    public AccountData AccountData { get; set; }
}