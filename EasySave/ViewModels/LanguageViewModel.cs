using System.Text.Json;
using EasySave.Models;

namespace EasySave.ViewModels;

/// <summary>
/// Class responsible for managing the translations of the application 
/// based on a given dictionary JSON file and the current language set in the config
/// </summary>
public class LanguageViewModel  
{
    private static LanguageViewModel? _instance;
    private static readonly object _lock = new();

    public static LanguageViewModel GetInstance(string dictionaryPath)
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                _instance ??= new LanguageViewModel(dictionaryPath);
            }
        }
        return _instance;
    }

    private readonly string _dictionaryPath;
    private Dictionary<string, Dictionary<Languages, string>> _dictionary;

    private readonly Config _conf;
    private Languages _currentLanguage;

    public event Action? LanguageChanged;

    public string T_error_loading_dictionary => this.GetTranslation("error_loading_dictionary");

    /// <summary>
    /// Constructor that initializes the dictionary and loads it from the 
    /// given JSON file path, also sets the current language based on the config
    /// </summary>
    private LanguageViewModel(string dictionaryPath) 
    {
        
        _dictionaryPath = dictionaryPath;
        _dictionary = [];
        _conf = Config.GetInstance();
        _currentLanguage = _conf.Language;
        LoadDictionary();
    }

    /// <summary>
    /// Method to change the current language, it updates the _currentLanguage 
    /// field and also updates the Language property of the Config class, then 
    /// it saves the updated configuration
    /// </summary>
    public void SetLanguage(Languages language) 
    {
        _currentLanguage = language;
        _conf.Language = language;
        _conf.SaveConfig();
        LanguageChanged?.Invoke();
    }

    public Languages GetCurrentLanguage()
    {
        return _currentLanguage;
    }

    /// <summary>
    /// Method to get the translation of a given word based on the current language,
    /// it looks up the word in the dictionary and returns the corresponding translation, 
    /// if the word or the translation is not found, it returns the original word
    /// </summary>
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

    /// <summary>
    /// Private method to load the dictionary from the JSON file, it reads the file
    /// content and deserializes it into the _dictionary field, if there is an error
    /// during loading, it initializes an empty dictionary
    /// </summary>
    private void LoadDictionary()
    {
        try
        {
            if (File.Exists(_dictionaryPath))
            {
                //var jsonContent = File.ReadAllText(_dictionaryPath);
                //Dictionary<string, Dictionary<Languages, string>>? loadedDictionary = JsonSerializer.Deserialize<Dictionary<string, Dictionary<Languages, string>>>(jsonContent);
                
                if (loadedDictionary != null)
                {
                    _dictionary = loadedDictionary;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{T_error_loading_dictionary}{ex.Message}");
            _dictionary = [];
        }
    }
}
