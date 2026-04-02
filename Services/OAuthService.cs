using Newtonsoft.Json.Linq;

namespace SDPIntegration.Services
{
    public class OAuthService
    {
        // Line 7: Simplified 'new'
        private readonly HttpClient _httpClient = new();

        public async Task<string> GetAccessTokenAsync(
            string clientId,
            string clientSecret,
            string refreshToken)
        {
            // Line 14: Simplified Collection Expression
            var requestData = new FormUrlEncodedContent([
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("grant_type", "refresh_token")
            ]);

            var response = await _httpClient.PostAsync(
                "https://accounts.zoho.in/oauth/v2/token",
                requestData);

            var json = await response.Content.ReadAsStringAsync();
            var obj = JObject.Parse(json);

            return obj["access_token"]?.ToString()!;
        }
    }
}