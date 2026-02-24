using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using EasySave.ViewModels;

namespace EasySave;

/// <summary>
/// Given a view model, returns the corresponding view if possible.
/// </summary>
[RequiresUnreferencedCode(
    "Default implementation of ViewLocator involves reflection which may be trimmed away.",
    Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
public class ViewLocator : IDataTemplate
{
    /// <summary>
    /// Method that builds a view based on the provided view model, it uses reflection
    /// to find a corresponding view class by replacing "ViewModel" with "View" in the 
    /// view model's type name, and if a matching view class is found, it creates an 
    /// instance of it and returns it as a Control. If no matching view class is found,
    /// it returns a TextBlock indicating that the view was not found.
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public Control? Build(object? param)    
    {
        if (param is null)
            return null;

        var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    /// <summary>
    /// Method that checks if the provided data is a view model, it returns
    /// true if the data is an instance of ViewModelBase, and false otherwise. 
    /// This is used to determine if the ViewLocator can handle the given data
    /// and build a corresponding view for it.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool Match(object? data) 
    {
        return data is ViewModelBase;
    }
}