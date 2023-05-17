using System.Text;

namespace LeagueToolkit.Core;

public class LeagueRemoteApi : ILeagueApi
{
    private readonly string _baseUrl;
    private readonly HttpClient _client;

    public LeagueRemoteApi(string authToken, int remotePort)
    {
        _baseUrl = $"https://127.0.0.1:{remotePort}";

        _client = new(new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        });
        
        _client.DefaultRequestHeaders.Authorization = new("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"riot:{authToken}")));

        _client.DefaultRequestHeaders.Add("Accept", "application/json");
        _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.2; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) LeagueOfLegendsClient/13.9.508.9888 (CEF 91) Safari/537.36");
        _client.DefaultRequestHeaders.Add("X-Riot-Source", "rcp-fe-lol-profiles");
        _client.DefaultRequestHeaders.Add("Referer", $"{_baseUrl}/index.html");
    }

    public async Task<string> MakeRequestAsync(HttpMethod method, string endpoint, HttpContent httpContent = null)
    {
        HttpRequestMessage requestMessage = new(method, $"{_baseUrl}{endpoint}");

        if (httpContent != null)
            requestMessage.Content = httpContent;

        HttpResponseMessage result = await _client.SendAsync(requestMessage);
        
        return await result.Content.ReadAsStringAsync();
    }
}