using System.Text.Json;
using EasySave.Models;

namespace EasySave.ViewModels;

public class LanguageViewModel
{
    private readonly string _dictionaryPath;
    private Dictionary<string, Dictionary<Languages, string>> _dictionary;
    private Languages _currentLanguage;

    public LanguageViewModel(string dictionaryPath)
    {
        _dictionaryPath = dictionaryPath;
        _dictionary = new Dictionary<string, Dictionary<Languages, string>>();
        _currentLanguage = Languages.EN; // Langue par défaut
        _loadDictionary();
    }

    public void SetLanguage(Languages language)
    {
        _currentLanguage = language;
    }

    public Languages GetCurrentLanguage()
    {
        return _currentLanguage;
    }

    public string GetTranslation(string word)
    {
        if (_dictionary.TryGetValue(word, out var translations))
        {
            if (translations.TryGetValue(_currentLanguage, out string? translation))
            {
                return translation;
            }
        }
        return word;
    }

    private void _loadDictionary()
    {
        try
        {
            if (File.Exists(_dictionaryPath))
            {
                string jsonContent = File.ReadAllText(_dictionaryPath);
                var loadedDictionary = JsonSerializer.Deserialize<Dictionary<string, Dictionary<Languages, string>>>(jsonContent);
                
                if (loadedDictionary != null)
                {
                    _dictionary = loadedDictionary;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading dictionary: {ex.Message}");
            _dictionary = new Dictionary<string, Dictionary<Languages, string>>();
        }
    }
}