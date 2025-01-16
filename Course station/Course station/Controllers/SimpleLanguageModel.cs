using System;
using System.Collections.Generic;
using System.Linq;
using Course_station.Models;
using Microsoft.Extensions.Logging;

public class SimpleLanguageModel
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<SimpleLanguageModel> _logger;

    public SimpleLanguageModel(ApplicationDbContext dbContext, ILogger<SimpleLanguageModel> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public List<Course> SuggestCourses(string userInput)
    {
        try
        {
            // Simple keyword-based matching for course suggestions
            var keywords = userInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var courses = _dbContext.Courses
                .Where(course => keywords.Any(keyword => course.Title != null && course.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            return courses ?? new List<Course>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error suggesting courses: {ex.Message}");
            return new List<Course>();
        }
    }

    public List<Instructor> SuggestInstructors(string userInput)
    {
        try
        {
            // Simple keyword-based matching for instructor suggestions
            var keywords = userInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var instructors = _dbContext.Instructors
                .Where(instructor => keywords.Any(keyword => instructor.InstructorName != null && instructor.InstructorName.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            return instructors ?? new List<Instructor>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error suggesting instructors: {ex.Message}");
            return new List<Instructor>();
        }
    }
}
