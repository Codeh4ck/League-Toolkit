using Spectre.Console;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LeagueToolkit.Core;
using LeagueToolkit.SoloQueueReveal.Models;

namespace LeagueToolkit.SoloQueueReveal.Utilities;

public static class LeagueUtilities
{
    public static async Task<QueueMap> GetCurrentSummonerRankedStats(ILeagueApi remoteApi, UserInfoActual userInfoActual)
    {
        string rankedStatisticsJson =
            await remoteApi.MakeRequestAsync(HttpMethod.Get, Endpoints.GetRankedStatsEndpoint);

        if (!string.IsNullOrEmpty(rankedStatisticsJson)) return ParseQueueMap(rankedStatisticsJson);

        ConsoleUtility.WriteColorLine(Color.Red,
            $"Could not fetch ranked statistics of {userInfoActual.AccountData.SummonerName}", true);
        
        return null;
    }

    public static async Task<QueueMap> GetSummonerRankedStats(ILeagueApi remoteApi, string summonerName)
    {
        string summonerProfileJson = await remoteApi.MakeRequestAsync(HttpMethod.Get,
            $"{Endpoints.GetLolSummonerEndpoint}?name={Uri.EscapeDataString(summonerName)}");

        if (string.IsNullOrEmpty(summonerProfileJson))
        {
            ConsoleUtility.WriteColorLine(Color.Red, $"Could not fetch summoner profile of {summonerName}.", true);
            return null;
        }

        JObject jObjectSummonerProfile = JObject.Parse(summonerProfileJson);
        JToken jTokenPuuid = jObjectSummonerProfile["puuid"];

        if (jTokenPuuid == null) return null;
        
        bool canParsePuuid = Guid.TryParse(jTokenPuuid.ToString(), out Guid puuid);
        if (!canParsePuuid)
        {
            ConsoleUtility.WriteColorLine(Color.Red, $"Could not parse PUUID of {summonerName}.", true);
            return null;
        }

        string summonerRankedStatsJson =
            await remoteApi.MakeRequestAsync(HttpMethod.Get, $"{Endpoints.GetLolSummonerRankedStatsEndpoint}/{puuid:D}");

        if (!string.IsNullOrEmpty(summonerRankedStatsJson)) return ParseQueueMap(summonerRankedStatsJson);
        
        ConsoleUtility.WriteColorLine(Color.Red, $"Could not fetch ranked stats of {summonerName}.", true);
        return null;
    }

    private static QueueMap ParseQueueMap(string json)
    {
        JObject jObject = JObject.Parse(json);
        JToken jToken = jObject["queueMap"];

        return jToken == null ? null : JsonConvert.DeserializeObject<QueueMap>(jToken.ToString());
    }
}