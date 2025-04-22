using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class ChatbotService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ChatbotService> _logger;

    public ChatbotService(HttpClient httpClient, ILogger<ChatbotService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> GenerateQuestionAsync(string text)
    {
        try
        {
            // Prepare the request payload
            var payload = new { text };
            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // Call the Python API
            var response = await _httpClient.PostAsync("http://127.0.0.1:8000/generate-question", content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to call Python API. Status Code: {response.StatusCode}");
                return "Error: Unable to generate a question.";
            }

            // Parse the response
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<QuestionResponse>(responseContent);

            return result?.Question ?? "Error: No question generated.";
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GenerateQuestionAsync: {ex.Message}");
            return "Error: An exception occurred while generating the question.";
        }
    }

    private class QuestionResponse
    {
        public string Question { get; set; }
    }
}
