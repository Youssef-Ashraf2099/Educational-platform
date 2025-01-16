using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ChatbotController : ControllerBase
{
    private readonly ChatbotService _chatbotService;

    public ChatbotController(ChatbotService chatbotService)
    {
        _chatbotService = chatbotService;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] string userInput)
    {
        var response = await _chatbotService.GetChatbotResponse(userInput);
        return Ok(response);
    }

    [HttpGet("recommend-courses")]
    public IActionResult RecommendCourses([FromQuery] string userInput)
    {
        var courses = _chatbotService.RecommendCourses(userInput);
        return Ok(courses);
    }

    [HttpGet("recommend-instructors")]
    public IActionResult RecommendInstructors([FromQuery] string userInput)
    {
        var instructors = _chatbotService.RecommendInstructors(userInput);
        return Ok(instructors);
    }
}
