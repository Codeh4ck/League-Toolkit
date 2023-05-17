using Newtonsoft.Json;

namespace LeagueToolkit.SoloQueueReveal.Models;

public class RankedStats
{
    [JsonProperty(PropertyName = "tier")]
    public string Tier { get; set; }
    [JsonProperty(PropertyName = "highestTier")]
    public string HighestTier { get; set; }
    [JsonProperty(PropertyName = "division")]
    public string Division { get; set; }
    [JsonProperty(PropertyName = "highestDivision")]
    public string HighestDivision { get; set; }
    [JsonProperty(PropertyName = "isProvisional")]
    public bool IsProvisional { get; set; }
    [JsonProperty(PropertyName = "leaguePoints")]
    public int LeaguePoints { get; set; }
}