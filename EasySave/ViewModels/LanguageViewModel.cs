namespace EasySave.ViewModels;

/// <summary>
/// Gère le système de traduction multi-langues de l'application
/// </summary>
public class LanguageViewModel
{
    private readonly string _dictionaryPath;
    private Dictionary<string, Dictionary<string, string>> _dictionary;
    private string _currentLanguage;

    /// <summary>
    /// Initialise le gestionnaire de langue avec le dictionnaire JSON
    /// </summary>
    /// <param name="dictionaryPath">Chemin vers le fichier dictionary.json</param>
    public LanguageViewModel(string dictionaryPath)
    {
        _dictionaryPath = dictionaryPath;
        _dictionary = new Dictionary<string, Dictionary<string, string>>();
        _currentLanguage = "en";
        _loadDictionary();
    }

    /// <summary>
    /// Change la langue active
    /// </summary>
    /// <param name="language">Code de langue (en, fr)</param>
    public void SetLanguage(string language)
    {
        _currentLanguage = language.ToLower();
    }

    /// <summary>
    /// Retourne la langue actuellement active
    /// </summary>
    public string GetCurrentLanguage()
    {
        return _currentLanguage;
    }

    /// <summary>
    /// Récupère la traduction d'une clé dans la langue active
    /// </summary>
    /// <param name="word">Clé de traduction</param>
    /// <returns>Texte traduit ou la clé si non trouvée</returns>
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
    /// Charge le dictionnaire depuis le fichier JSON
    /// </summary>
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