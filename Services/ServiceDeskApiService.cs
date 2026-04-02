using Newtonsoft.Json;

namespace SDPIntegration.Services
{
    public class ServiceDeskApiService
    {
        // Fixed: Line 7
        private readonly HttpClient _client = new();

        public async Task<string> CreateTicketAsync(
            string baseUrl,
            string portal,
            string token)
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", $"Zoho-oauthtoken {token}");
            _client.DefaultRequestHeaders.Add("Accept", "application/vnd.manageengine.sdp.v3+json");

            var requestBody = new
            {
                request = new
                {
                    subject = "API Ticket from C#",
                    description = "<div>Created from C# integration</div>",
                    requester = new { name = "Diana Patrick" },
                    template = new { name = "Default Request" },
                    request_type = new { name = "Incident" },
                    priority = new { name = "High" },
                    status = new { name = "Open" },
                    mode = new { name = "Web Form" }
                }
            };

            // Fixed: Line 57 (Collection expression [])
            var formData = new FormUrlEncodedContent([
                new KeyValuePair<string, string>(
                    "input_data",
                    JsonConvert.SerializeObject(requestBody))
            ]);

            var response = await _client.PostAsync(
                $"{baseUrl}/app/{portal}/api/v3/requests",
                formData);

            var result = await response.Content.ReadAsStringAsync();

            Console.WriteLine("STATUS: " + response.StatusCode);
            Console.WriteLine("RESPONSE: " + result);

            return result;
        }
    }
}