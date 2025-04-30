using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

public class ChatbotService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ChatbotService> _logger;
    private readonly string _apiBaseUrl;

    public ChatbotService(HttpClient httpClient, ILogger<ChatbotService> logger, IConfiguration config)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiBaseUrl = config["ChatbotApi:BaseUrl"] ?? "http://localhost:8000"; // fallback if not set
    }

    public async Task<string> GenerateQuestionAsync(string text)
    {
        try
        {
            var payload = new { text = text };
            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var endpoint = $"{_apiBaseUrl}/generate/general";

            // Add detailed logging
            _logger.LogInformation($"Calling endpoint: {endpoint}");
            _logger.LogInformation($"Request payload: {jsonPayload}");

            var response = await _httpClient.PostAsync(endpoint, content);

            // Read response content regardless of status code
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"API Response: {responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"API Error: {response.StatusCode} - {responseContent}");
                throw new Exception($"API Error: {response.StatusCode}");
            }

            try
            {
                var result = JsonSerializer.Deserialize<QuestionResponse>(responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result?.Question ?? "Error: Empty question in response";
            }
            catch (JsonException ex)
            {
                _logger.LogError($"JSON Parsing Error: {ex.Message}, Content: {responseContent}");
                throw new Exception($"Error parsing API response: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.ToString()}");
            throw; // Re-throw to let the controller handle it
        }
    }

    public async Task<MCQResponse> GenerateMCQAsync(string text)
    {
        return await CallApi<MCQResponse>($"{_apiBaseUrl}/generate/mcq", new { text = text });
    }

    public async Task<TrueFalseResponse> GenerateTrueFalseAsync(string text)
    {
        return await CallApi<TrueFalseResponse>($"{_apiBaseUrl}/generate/truefalse", new { text = text });
    }

    public async Task<string> GenerateCodingQuestionAsync(string text)
    {
        var response = await CallApi<CodingQuestionResponse>($"{_apiBaseUrl}/generate/coding", new { code = text });
        return response.Question;
    }

    private async Task<T> CallApi<T>(string url, object payload)
    {
        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        _logger.LogInformation($"Calling endpoint: {url}");
        _logger.LogInformation($"Request payload: {jsonPayload}");

        try
        {
            var response = await _httpClient.PostAsync(url, content);

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"API Response: {responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"API Error: {response.StatusCode} - {responseContent}");
                throw new Exception($"API Error: {response.StatusCode}");
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<T>(responseContent, options) ??
                throw new Exception("Failed to deserialize response");
        }
        catch (Exception ex)
        {
            _logger.LogError($"API Call Exception: {ex.Message}");
            throw;
        }
    }

    public class QuestionResponse
    {
        public string Question { get; set; } = string.Empty;
    }

    public class MCQResponse
    {
        public string Question { get; set; } = string.Empty;
        public string[] Options { get; set; } = Array.Empty<string>();
        public string CorrectAnswer { get; set; } = string.Empty;
    }

    public class TrueFalseResponse
    {
        public string Statement { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
    }

    public class CodingQuestionResponse
    {
        public string Question { get; set; } = string.Empty;
    }
}