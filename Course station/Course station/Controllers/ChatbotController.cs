using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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

    public class TextInputModel
    {
        public string Text { get; set; }
    }

    [HttpPost("generate-question")]
    public async Task<IActionResult> GenerateQuestion([FromBody] TextInputModel input)
    {
        try
        {
            if (input == null || string.IsNullOrEmpty(input.Text))
            {
                return BadRequest("Input text cannot be empty.");
            }

            var question = await _chatbotService.GenerateQuestionAsync(input.Text);
            return Ok(new { question });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GenerateQuestion: {ex.Message}");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("generate-mcq")]
    public async Task<IActionResult> GenerateMCQ([FromBody] TextInputModel input)
    {
        try
        {
            if (input == null || string.IsNullOrEmpty(input.Text))
            {
                return BadRequest("Input text cannot be empty.");
            }

            var mcq = await _chatbotService.GenerateMCQAsync(input.Text);
            return Ok(mcq);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GenerateMCQ: {ex.Message}");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("generate-truefalse")]
    public async Task<IActionResult> GenerateTrueFalse([FromBody] TextInputModel input)
    {
        try
        {
            if (input == null || string.IsNullOrEmpty(input.Text))
            {
                return BadRequest("Input text cannot be empty.");
            }

            var trueFalse = await _chatbotService.GenerateTrueFalseAsync(input.Text);
            return Ok(trueFalse);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GenerateTrueFalse: {ex.Message}");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("generate-coding")]
    public async Task<IActionResult> GenerateCoding([FromBody] TextInputModel input)
    {
        try
        {
            if (input == null || string.IsNullOrEmpty(input.Text))
            {
                return BadRequest("Input code cannot be empty.");
            }

            var codingQuestion = await _chatbotService.GenerateCodingQuestionAsync(input.Text);
            return Ok(new { question = codingQuestion });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GenerateCoding: {ex.Message}");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}