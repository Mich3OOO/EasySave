using CommunityToolkit.Mvvm.ComponentModel;

namespace EasySave.ViewModels;

/// <summary>
/// Abstract base class for all view models in the application, it inherits
/// from ObservableObject to provide property change notifications and can
/// be extended by specific view models for different views (e.g., 
/// BackupViewModel, SettingsViewModel, etc.). This class can also include
/// common functionality or properties that are shared across multiple view models.
/// </summary>
public abstract class ViewModelBase : ObservableObject  
{
}