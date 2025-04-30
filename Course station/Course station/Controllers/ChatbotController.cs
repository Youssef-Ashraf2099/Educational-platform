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
        _logger.LogInformation($"GenerateQuestion endpoint called");

        try
        {
            if (input == null || string.IsNullOrEmpty(input.Text))
            {
                _logger.LogWarning("Input text is empty");
                return BadRequest(new { error = "Input text cannot be empty." });
            }

            _logger.LogInformation($"Calling service with text: {input.Text}");
            var question = await _chatbotService.GenerateQuestionAsync(input.Text);

            _logger.LogInformation($"Service returned question: {question}");
            return Ok(new { question });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GenerateQuestion: {ex.Message}");
            // Return a more detailed error response for debugging
            return StatusCode(500, new
            {
                error = "An error occurred while processing your request.",
                details = ex.Message
            });
        }
    }

    [HttpPost("generate-mcq")]
    public async Task<IActionResult> GenerateMCQ([FromBody] TextInputModel input)
    {
        _logger.LogInformation($"GenerateMCQ endpoint called");

        try
        {
            if (input == null || string.IsNullOrEmpty(input.Text))
            {
                _logger.LogWarning("Input text is empty");
                return BadRequest(new { error = "Input text cannot be empty." });
            }

            _logger.LogInformation($"Calling service with text: {input.Text}");
            var mcq = await _chatbotService.GenerateMCQAsync(input.Text);

            _logger.LogInformation($"Service returned MCQ with question: {mcq.Question}");
            return Ok(new
            {
                question = mcq.Question,
                options = mcq.Options,
                correctAnswer = mcq.CorrectAnswer
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GenerateMCQ: {ex.Message}");
            return StatusCode(500, new
            {
                error = "An error occurred while processing your request.",
                details = ex.Message
            });
        }
    }

    [HttpPost("generate-truefalse")]
    public async Task<IActionResult> GenerateTrueFalse([FromBody] TextInputModel input)
    {
        _logger.LogInformation($"GenerateTrueFalse endpoint called");

        try
        {
            if (input == null || string.IsNullOrEmpty(input.Text))
            {
                _logger.LogWarning("Input text is empty");
                return BadRequest(new { error = "Input text cannot be empty." });
            }

            _logger.LogInformation($"Calling service with text: {input.Text}");
            var trueFalse = await _chatbotService.GenerateTrueFalseAsync(input.Text);

            _logger.LogInformation($"Service returned True/False statement: {trueFalse.Statement}");
            return Ok(new
            {
                statement = trueFalse.Statement,
                correctAnswer = trueFalse.CorrectAnswer
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GenerateTrueFalse: {ex.Message}");
            return StatusCode(500, new
            {
                error = "An error occurred while processing your request.",
                details = ex.Message
            });
        }
    }

    [HttpPost("generate-coding")]
    public async Task<IActionResult> GenerateCoding([FromBody] TextInputModel input)
    {
        _logger.LogInformation($"GenerateCoding endpoint called");

        try
        {
            if (input == null || string.IsNullOrEmpty(input.Text))
            {
                _logger.LogWarning("Input text is empty");
                return BadRequest(new { error = "Input code cannot be empty." });
            }

            _logger.LogInformation($"Calling service with text: {input.Text}");
            var codingQuestion = await _chatbotService.GenerateCodingQuestionAsync(input.Text);

            _logger.LogInformation($"Service returned coding question: {codingQuestion}");
            return Ok(new { question = codingQuestion });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GenerateCoding: {ex.Message}");
            return StatusCode(500, new
            {
                error = "An error occurred while processing your request.",
                details = ex.Message
            });
        }
    }
}