using System;
using System.Collections.Generic;
using System.Linq;

public class SimpleLanguageModel
{
    private Dictionary<string, List<string>> _wordDictionary;

    public SimpleLanguageModel()
    {
        _wordDictionary = new Dictionary<string, List<string>>();
    }

    public void Train(string text)
    {
        var words = text.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < words.Length - 1; i++)
        {
            var word = words[i].ToLower();
            var nextWord = words[i + 1].ToLower();

            if (!_wordDictionary.ContainsKey(word))
            {
                _wordDictionary[word] = new List<string>();
            }

            _wordDictionary[word].Add(nextWord);
        }
    }

    public string GenerateText(string seed, int length)
    {
        var random = new Random();
        var currentWord = seed.ToLower();
        var result = new List<string> { currentWord };

        for (int i = 0; i < length - 1; i++)
        {
            if (!_wordDictionary.ContainsKey(currentWord) || _wordDictionary[currentWord].Count == 0)
            {
                break;
            }

            var nextWords = _wordDictionary[currentWord];
            currentWord = nextWords[random.Next(nextWords.Count)];
            result.Add(currentWord);
        }

        return string.Join(" ", result);
    }
}
