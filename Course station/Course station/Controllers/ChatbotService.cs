using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Course_station.Models;
using Newtonsoft.Json;

public class ChatbotService
{
    private readonly HttpClient _httpClient;
    private readonly ApplicationDbContext _dbContext;

    public ChatbotService(HttpClient httpClient, ApplicationDbContext dbContext)
    {
        _httpClient = httpClient;
        _dbContext = dbContext;
    }

    public async Task<string> GetChatbotResponse(string userInput)
    {
        var requestBody = new
        {
            prompt = userInput,
            max_tokens = 150
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("http://172.17.176.202:5000/api/chatbot", content); // Flask server endpoint
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<dynamic>(responseBody);

        return result.choices[0].text;
    }

    public List<Course> RecommendCourses(string userInput)
    {
        // Implement logic to recommend courses based on user input
        return _dbContext.Courses.ToList();
    }

    public List<Instructor> RecommendInstructors(string userInput)
    {
        // Implement logic to recommend instructors based on user input
        return _dbContext.Instructors.ToList();
    }
}
