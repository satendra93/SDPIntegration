using System.Text.Json;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main()
    {
        // Load configuration
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var meConfig = config.GetSection("ManageEngine");

        string baseUrl = meConfig["BaseUrl"]!;
        string clientId = meConfig["ClientId"]!;
        string clientSecret = meConfig["ClientSecret"]!;
        string refreshToken = meConfig["RefreshToken"]!;

        // STEP 1: Get Access Token
        using var tokenClient = new HttpClient();

        var tokenContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("refresh_token", refreshToken),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("grant_type", "refresh_token")
        });

        var tokenResponse = await tokenClient.PostAsync(
            "https://accounts.zoho.in/oauth/v2/token",
            tokenContent);

        string tokenResult = await tokenResponse.Content.ReadAsStringAsync();

        using var tokenJson = JsonDocument.Parse(tokenResult);

        if (!tokenJson.RootElement.TryGetProperty("access_token", out var tokenElement))
        {
            Console.WriteLine("Token Error:");
            Console.WriteLine(tokenResult);
            return;
        }

        string accessToken = tokenElement.GetString()!;
        Console.WriteLine("Access Token: " + accessToken);

        // STEP 2: FINAL CORRECT REQUEST URL
        string requestUrl = $"{baseUrl}/api/v3/requests";
        Console.WriteLine("Request URL: " + requestUrl);

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Zoho-oauthtoken {accessToken}");
        client.DefaultRequestHeaders.Add("Accept", "application/vnd.manageengine.sdp.v3+json");

        // STEP 3: Payload
        var requestObject = new
        {
            request = new
            {
                subject = "Need an External Monitor",
                description = "<div>Provide me an External Monitor</div>",

                requester = new
                {
                    name = "Diana Patrick"
                },

                template = new
                {
                    name = "Default Request"
                },

                request_type = new
                {
                    name = "Incident"
                },

                priority = new
                {
                    name = "High"
                },

                status = new
                {
                    name = "Open"
                },

                mode = new
                {
                    name = "Web Form"
                }
            }
        };

        var formData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>(
                "input_data",
                JsonSerializer.Serialize(requestObject))
        });

        // STEP 4: Call API
        var response = await client.PostAsync(requestUrl, formData);
        string result = await response.Content.ReadAsStringAsync();

        Console.WriteLine("STATUS: " + response.StatusCode);
        Console.WriteLine("API Response:");
        Console.WriteLine(result);
    }
}