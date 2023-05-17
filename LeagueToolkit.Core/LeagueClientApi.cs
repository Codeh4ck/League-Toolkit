using System.Text;

namespace LeagueToolkit.Core;

public class LeagueClientApi : ILeagueApi
{
    private readonly string _baseUrl;
    private readonly HttpClient _client;

    public LeagueClientApi(string authToken, int authPort)
    {
        _baseUrl = $"https://127.0.0.1:{authPort}";

        _client = new(new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        });
        
        _client.DefaultRequestHeaders.Authorization = new("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"riot:{authToken}")));

        _client.DefaultRequestHeaders.Add("Accept", "application/json");
        _client.DefaultRequestHeaders.Add("User-Agent", "LeagueOfLegendsClient");
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