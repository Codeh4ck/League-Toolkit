using Newtonsoft.Json;

namespace LeagueToolkit.SoloQueueReveal.Utilities;

public static class StringExtensions
{
    public static string PrettifyJson(this string json)
    {
        using StringReader stringReader = new(json);
        using StringWriter stringWriter = new();
        
        JsonTextReader jsonReader = new(stringReader);
        JsonTextWriter jsonWriter = new(stringWriter) { Formatting = Formatting.Indented };
        
        jsonWriter.WriteToken(jsonReader);
        
        return stringWriter.ToString();
    }
}