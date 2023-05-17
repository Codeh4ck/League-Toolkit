using Newtonsoft.Json;

namespace LeagueToolkit.SoloQueueReveal.Models;

public class Participants
{
    [JsonProperty(PropertyName = "participants")]
    public List<TeamMember> TeamMembers { get; set; }
}


public class TeamMember
{
    [JsonProperty(PropertyName = "activePlatform")]
    public object ActivePlatform { get; set; }
    
    [JsonProperty(PropertyName = "cid")]
    public string Cid { get; set; }
    
    [JsonProperty(PropertyName = "game_name")]
    public string GameName { get; set; }
    
    [JsonProperty(PropertyName = "game_tag")]
    public string GameTag { get; set; }
    
    [JsonProperty(PropertyName = "muted")]
    public bool Muted { get; set; }
    
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }
    
    [JsonProperty(PropertyName = "pid")]
    public string Pid { get; set; }
    
    [JsonProperty(PropertyName = "puuid")]
    public string PlayerUuid { get; set; }
    
    [JsonProperty(PropertyName = "region")]
    public string Region { get; set; }
}