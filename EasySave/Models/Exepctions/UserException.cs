using EasySave.ViewModels;

namespace EasySave.Models.Exepctions;

public class UserException:Exception
{
    public UserException(string? errorRefference):base(LanguageViewModel
        .GetInstance(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "Dictionary.json"))
        .GetTranslation(errorRefference)) // Get transaltion from error to display to user
    {
        
    }
    
}