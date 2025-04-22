using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

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

    [HttpPost("generate-question")]
    public async Task<IActionResult> GenerateQuestion([FromBody] string text)
    {
        try
        {
            if (string.IsNullOrEmpty(text))
            {
                return BadRequest("Input text cannot be empty.");
            }

            var question = await _chatbotService.GenerateQuestionAsync(text);
            return Ok(new { question });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GenerateQuestion: {ex.Message}");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
