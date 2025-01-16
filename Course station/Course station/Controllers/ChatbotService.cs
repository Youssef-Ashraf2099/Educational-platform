using System;
using System.Collections.Generic;
using System.Linq;
using Course_station.Models;
using Microsoft.Extensions.Logging;

public class ChatbotService
{
    private readonly SimpleLanguageModel _languageModel;
    private readonly ILogger<ChatbotService> _logger;

    public ChatbotService(SimpleLanguageModel languageModel, ILogger<ChatbotService> logger)
    {
        _languageModel = languageModel;
        _logger = logger;
    }

    public string GetResponse(string userInput)
    {
        try
        {
            // Get course suggestions based on user input
            var courseSuggestions = _languageModel.SuggestCourses(userInput);
            var instructorSuggestions = _languageModel.SuggestInstructors(userInput);

            // Generate a response based on the suggestions
            string response = "";

            if (courseSuggestions.Any())
            {
                response += "Here are some courses you might be interested in:\n";
                foreach (var course in courseSuggestions)
                {
                    response += $"- {course.Title}\n";
                }
            }

            if (instructorSuggestions.Any())
            {
                response += "\nHere are some instructors you might want to check out:\n";
                foreach (var instructor in instructorSuggestions)
                {
                    response += $"- {instructor.InstructorName}\n";
                }
            }

            if (string.IsNullOrEmpty(response))
            {
                response = "I couldn't find any courses or instructors matching your query. Please try again with different keywords.";
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generating chatbot response: {ex.Message}");
            return "An error occurred while processing your request. Please try again later.";
        }
    }
}