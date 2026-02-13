using System.Text.Json;
using EasySave.Models;

namespace EasySave.ViewModels;

public class LanguageViewModel  // Class responsible for managing the translations of the application based on a given dictionary JSON file and the current language set in the config
{
    // The dictionary is a nested dictionary where the first key is the word to translate, and the value is another dictionary where the key is the language and the value is the translation of the word in that language
    private readonly string _dictionaryPath;
    private Dictionary<string, Dictionary<Languages, string>> _dictionary;
    private Languages _currentLanguage;
    Config _conf ;

    public LanguageViewModel(string dictionaryPath) // Constructor that initializes the dictionary and loads it from the given JSON file path, also sets the current language based on the config
    {
        _conf = Config.S_GetInstance();
        _dictionaryPath = dictionaryPath;
        _dictionary = new Dictionary<string, Dictionary<Languages, string>>();
        _currentLanguage = _conf.Language;
        _loadDictionary();
    }

    public void SetLanguage(Languages language) // Method to change the current language, it updates the _currentLanguage field and also updates the Language property of the Config class, then it saves the updated configuration
    {
        _currentLanguage = language;
        _conf.Language = language;
        _conf.SaveConfig();
    }

    public Languages GetCurrentLanguage()   // Method to get the current language
    {
        return _currentLanguage;
    }

    public string GetTranslation(string word)   // Method to get the translation of a given word based on the current language, it looks up the word in the dictionary and returns the corresponding translation, if the word or the translation is not found, it returns the original word
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

    private void _loadDictionary()  // Private method to load the dictionary from the JSON file, it reads the file content and deserializes it into the _dictionary field, if there is an error during loading, it initializes an empty dictionary
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
        catch (Exception ex)    // Handle any exceptions that occur during the loading of the dictionary, such as file not found, invalid JSON format, etc., and initialize an empty dictionary in case of error
        {
            Console.WriteLine($"Error loading dictionary: {ex.Message}");
            _dictionary = new Dictionary<string, Dictionary<Languages, string>>();
        }
    }
}