using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

public class AISampleService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _endpoint;
    private readonly string _apiKey;
    private readonly string _deploymentId;

    public AISampleService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _endpoint = _configuration["AzureOpenAI:Endpoint"];
        _apiKey = _configuration["AzureOpenAI:ApiKey"];
        _deploymentId = _configuration["AzureOpenAI:DeploymentId"];
    }

    public async Task<string> GetAIResponse(string prompt)
    {
        var requestBody = new
        {
            messages = new[]
            {
               new { role = "user", content = prompt }
            },
            max_tokens = 2000
        };

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_endpoint}/openai/deployments/{_deploymentId}/chat/completions?api-version=2023-07-01-preview")
        {
            Content = JsonContent.Create(requestBody)
        };

        request.Headers.Add("api-key", _apiKey);

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return $"Error: {response.StatusCode} - {errorContent}";
        }
    }
}