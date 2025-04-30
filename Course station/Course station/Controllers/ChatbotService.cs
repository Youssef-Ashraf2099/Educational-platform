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
    private readonly JsonSerializerOptions _jsonOptions;

    public ChatbotService(HttpClient httpClient, ILogger<ChatbotService> logger, IConfiguration config)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiBaseUrl = config["ChatbotApi:BaseUrl"] ?? "http://localhost:8000"; // fallback if not set
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // You might need to increase the timeout for API calls
        _httpClient.Timeout = TimeSpan.FromSeconds(10000);
    }

    public async Task<string> GenerateQuestionAsync(string text)
    {
        try
        {
            _logger.LogInformation($"GenerateQuestionAsync called with text: {text}");

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
                var result = JsonSerializer.Deserialize<QuestionResponse>(responseContent, _jsonOptions);
                if (result == null || string.IsNullOrEmpty(result.Question))
                {
                    _logger.LogWarning("Received empty question in response");
                    return "Error: Empty question in response";
                }
                return result.Question;
            }
            catch (JsonException ex)
            {
                _logger.LogError($"JSON Parsing Error: {ex.Message}, Content: {responseContent}");
                throw new Exception($"Error parsing API response: {ex.Message}");
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError($"HTTP Request Exception: {ex.ToString()}");
            throw new Exception($"Failed to connect to API: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.ToString()}");
            throw; // Re-throw to let the controller handle it
        }
    }

    public async Task<MCQResponse> GenerateMCQAsync(string text)
    {
        try
        {
            _logger.LogInformation($"GenerateMCQAsync called with text: {text}");
            var response = await CallApi<MCQResponse>($"{_apiBaseUrl}/generate/mcq", new { text = text });

            // Validate response data
            if (response.Options == null || response.Options.Length < 4)
            {
                _logger.LogWarning("MCQ response doesn't have enough options");
                // Provide default options if needed
                response.Options = response.Options ?? Array.Empty<string>();
                while (response.Options.Length < 4)
                {
                    var newOptions = new string[4];
                    Array.Copy(response.Options, newOptions, response.Options.Length);
                    for (int i = response.Options.Length; i < 4; i++)
                    {
                        newOptions[i] = $"Option {i + 1}";
                    }
                    response.Options = newOptions;
                }
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception in GenerateMCQAsync: {ex.Message}");
            throw;
        }
    }

    public async Task<TrueFalseResponse> GenerateTrueFalseAsync(string text)
    {
        try
        {
            _logger.LogInformation($"GenerateTrueFalseAsync called with text: {text}");
            return await CallApi<TrueFalseResponse>($"{_apiBaseUrl}/generate/truefalse", new { text = text });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception in GenerateTrueFalseAsync: {ex.Message}");
            throw;
        }
    }

    public async Task<string> GenerateCodingQuestionAsync(string text)
    {
        try
        {
            _logger.LogInformation($"GenerateCodingQuestionAsync called with text: {text}");
            var response = await CallApi<CodingQuestionResponse>($"{_apiBaseUrl}/generate/coding", new { code = text });
            return response.Question;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception in GenerateCodingQuestionAsync: {ex.Message}");
            throw;
        }
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

            var result = JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
            if (result == null)
            {
                throw new Exception("Failed to deserialize response");
            }

            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError($"HTTP Request Exception: {ex.ToString()}");
            throw new Exception($"Failed to connect to API: {ex.Message}", ex);
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