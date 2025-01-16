using System;
using System.Collections.Generic;
using System.Linq;
using Course_station.Models;
using Microsoft.Extensions.Logging;
//using FuzzySharp;
using System.Diagnostics;

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
            var lowerInput = userInput.ToLower();

            // Handle greetings
            if (IsGreeting(lowerInput))
            {
                return "Hello! How can I assist you today?";
            }

            // Handle common questions with fuzzy matching
            var matchedQuestion = FuzzyMatchCommonQuestion(lowerInput);
            if (!string.IsNullOrEmpty(matchedQuestion))
            {
                return HandleCommonQuestion(matchedQuestion);
            }

            // Get course and instructor suggestions with fuzzy matching
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

            // Fallback for unrecognized input
            if (!courseSuggestions.Any() && !instructorSuggestions.Any() && !IsGreeting(lowerInput) && string.IsNullOrEmpty(matchedQuestion))
            {
                response = "I'm here to help you find courses and instructors. Try asking something like, 'Can you recommend a course on machine learning?'";
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generating chatbot response: {ex.Message}");
            return "An error occurred while processing your request. Please try again later.";
        }
    }

    private bool IsGreeting(string input)
    {
        var greetings = new List<string> { "hello", "hi", "hey", "how are you", "good morning", "good afternoon", "good evening" };
        return greetings.Any(greeting => input.Contains(greeting));
    }

    private string FuzzyMatchCommonQuestion(string input)
    {
        var commonQuestions = new List<string>
    {
        "what courses do you offer",
        "what courses do you suggest",
        "what courses do you recommend",
        "what courses can i take",
        "who are the instructors",
        "can you suggest instructors",
        "can you recommend instructors",
        "tell me about the instructors",
        "what instructors do you have"
    };

        string bestMatch = null;
        int bestScore = int.MaxValue;

        foreach (var question in commonQuestions)
        {
            int distance = LevenshteinDistance(input, question);
            if (distance < bestScore)
            {
                bestScore = distance;
                bestMatch = question;
            }
        }

        // If the best score is below a threshold, return the matched question
        if (bestScore <= 5) // Adjust the threshold as needed
        {
            return bestMatch;
        }

        return null;
    }
    private string HandleCommonQuestion(string matchedQuestion)
    {
        if (matchedQuestion.Contains("course"))
        {
            // Handle course-related questions
            var courseSuggestions = _languageModel.SuggestCourses(""); // Empty input to get all courses
            if (courseSuggestions.Any())
            {
                string response = "We offer a wide range of courses. Here are some popular ones:\n";
                foreach (var course in courseSuggestions)
                {
                    response += $"- {course.Title}\n";
                }
                response += "You can ask me about a specific topic, like 'Can you recommend a course on machine learning?'";
                return response;
            }
            else
            {
                return "We currently don't have any courses available. Please check back later!";
            }
        }
        else if (matchedQuestion.Contains("instructor"))
        {
            // Handle instructor-related questions
            var instructorSuggestions = _languageModel.SuggestInstructors(""); // Empty input to get all instructors
            if (instructorSuggestions.Any())
            {
                string response = "Our instructors are industry experts. Here are some of them:\n";
                foreach (var instructor in instructorSuggestions)
                {
                    response += $"- {instructor.InstructorName}\n";
                }
                response += "You can ask me about instructors in a specific field, like 'Who teaches machine learning?'";
                return response;
            }
            else
            {
                return "We currently don't have any instructors listed. Please check back later!";
            }
        }

        // Default response for unrecognized common questions
        return "I'm here to help you find courses and instructors. Try asking something like, 'Can you recommend a course on machine learning?'";
    }

    private int LevenshteinDistance(string s, string t)
    {
        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        if (n == 0) return m;
        if (m == 0) return n;

        for (int i = 0; i <= n; d[i, 0] = i++) { }
        for (int j = 0; j <= m; d[0, j] = j++) { }

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }

        return d[n, m];
    }


}