using CommunityToolkit.Mvvm.ComponentModel;


namespace EasySave.ViewModels;

/// <summary>
/// Abstract base class for all view models in the application, it inherits from ObservableObject
/// to provide property change notifications and can be extended by specific view models for different views
/// This class can also include common functionality or properties that are shared across multiple view models.
/// </summary>
public abstract class ViewModelBase : ObservableObject
{
    public LanguageViewModel _languageViewModel { get; }
    public ViewModelBase()
    {
        var dictionaryPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "Dictionary.json");
        _languageViewModel = LanguageViewModel.GetInstance(dictionaryPath);
    }
}
