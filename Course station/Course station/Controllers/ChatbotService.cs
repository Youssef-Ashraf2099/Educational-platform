using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
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

        _httpClient.Timeout = TimeSpan.FromSeconds(10000);
    }

    public async Task<string> GenerateQuestionAsync(string text)
    {
        try
        {
            _logger.LogInformation($"GenerateQuestionAsync called with text: {text}");

            var payload = new { topic = text };
            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var endpoint = $"{_apiBaseUrl}/generate/general";
            var response = await _httpClient.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"API Response: {responseContent}");

            if (!response.IsSuccessStatusCode)
                throw new Exception($"API Error: {response.StatusCode}");

            var result = JsonSerializer.Deserialize<QuestionResponse>(responseContent, _jsonOptions);
            return result?.Question ?? "No question returned from API.";
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
            throw;
        }
    }

    public async Task<MCQResponse> GenerateMCQAsync(string text)
    {
        try
        {
            var response = await CallApi<MCQResponse>($"{_apiBaseUrl}/generate/mcq", new { topic = text });

            if (response.Options == null || response.Options.Length < 4)
            {
                response.Options ??= Array.Empty<string>();
                var paddedOptions = new List<string>(response.Options);
                while (paddedOptions.Count < 4)
                    paddedOptions.Add($"Option {paddedOptions.Count + 1}");

                response.Options = paddedOptions.ToArray();
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
            return await CallApi<TrueFalseResponse>($"{_apiBaseUrl}/generate/truefalse", new { topic = text });
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
            var response = await CallApi<CodingQuestionResponse>($"{_apiBaseUrl}/generate/coding", new { topic = text });
            return response.Question;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception in GenerateCodingQuestionAsync: {ex.Message}");
            throw;
        }
    }

    public async Task<string> GetResponse(string userInput)
    {
        try
        {
            var response = await CallApi<QuestionResponse>($"{_apiBaseUrl}/chat", new { message = userInput });
            return response.Question ?? "No response generated.";
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception in GetResponse: {ex.Message}");
            return "An error occurred while getting the response.";
        }
    }

    public async Task<List<string>> GetSuggestions(string prompt)
    {
        try
        {
            var response = await CallApi<SuggestionResponse>($"{_apiBaseUrl}/suggest", new { input = prompt });
            return response?.Suggestions ?? new List<string> { "No suggestions available." };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception in GetSuggestions: {ex.Message}");
            return new List<string> { "Error generating suggestions." };
        }
    }

    private async Task<T> CallApi<T>(string url, object payload)
    {
        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"API Error: {response.StatusCode} - {responseContent}");

            var result = JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
            if (result == null)
                throw new Exception("Failed to deserialize response");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"API Call Exception: {ex.Message}");
            throw;
        }
    }

    // Models for deserialization
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

    public class SuggestionResponse
    {
        public List<string> Suggestions { get; set; } = new();
    }
}
