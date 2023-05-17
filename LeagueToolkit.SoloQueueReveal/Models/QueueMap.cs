using Newtonsoft.Json;

namespace LeagueToolkit.SoloQueueReveal.Models;

public class QueueMap
{
    [JsonProperty(PropertyName = "RANKED_SOLO_5x5")]
    public RankedStats SoloQueue { get; set; }
    [JsonProperty(PropertyName = "RANKED_FLEX_SR")]
    public RankedStats FlexQueue { get; set; }
}