using Newtonsoft.Json;

namespace LeagueToolkit.SoloQueueReveal.Models;

public class UserInfo
{
    [JsonProperty(PropertyName = "userinfo")]
    public string Info { get; set; }
}