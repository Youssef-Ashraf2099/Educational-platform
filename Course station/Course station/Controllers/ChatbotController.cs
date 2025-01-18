using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class ChatbotController : ControllerBase
{
    private readonly ChatbotService _chatbotService;
    private readonly ILogger<ChatbotController> _logger;

    public ChatbotController(ChatbotService chatbotService, ILogger<ChatbotController> logger)
    {
        _chatbotService = chatbotService;
        _logger = logger;
    }

    [HttpPost("suggestions")]
    public IActionResult GetSuggestions([FromBody] string userInput)
    {
        try
        {
            if (string.IsNullOrEmpty(userInput))
            {
                return BadRequest("User input cannot be empty.");
            }

            var suggestions = _chatbotService.GetSuggestions(userInput);
            return Ok(suggestions);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in ChatbotController: {ex.Message}");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("chat")]
    public IActionResult Chat([FromBody] string userInput)
    {
        try
        {
            if (string.IsNullOrEmpty(userInput))
            {
                return BadRequest("User input cannot be empty.");
            }

            var response = _chatbotService.GetResponse(userInput);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in ChatbotController: {ex.Message}");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}