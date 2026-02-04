namespace EasySave.ViewModels;

public class LanguageViewModel
{
    private readonly string _dictionaryPath;
    private Dictionary<string, Dictionary<string, string>> _dictionary;
    private string _currentLanguage;

    public LanguageViewModel(string dictionaryPath)
    {
        _dictionaryPath = dictionaryPath;
        _dictionary = new Dictionary<string, Dictionary<string, string>>();
        _currentLanguage = "en"; // Langue par d�faut
        _loadDictionary();
    }

    public void SetLanguage(string language)
    {
        _currentLanguage = language.ToLower();
    }

    public string GetCurrentLanguage()
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
                var loadedDictionary = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(jsonContent);
                
                if (loadedDictionary != null)
                {
                    _dictionary = loadedDictionary;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading dictionary: {ex.Message}");
            _dictionary = new Dictionary<string, Dictionary<string, string>>();
        }
    }
}