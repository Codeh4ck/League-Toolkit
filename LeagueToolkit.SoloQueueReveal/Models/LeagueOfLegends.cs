using Newtonsoft.Json;

namespace LeagueToolkit.SoloQueueReveal.Models;

public class LeagueOfLegends
{
    [JsonProperty(PropertyName = "cpid")]
    public string Platform { get; set; }
}