using EasySave.Views;
using EasySave.ViewModels;
using Avalonia;
using System;

namespace EasySave;
internal class Program  // ReSharper disable once UnusedMember.Global
{
    [STAThread] // ReSharper disable once UnusedMember.Global
    public static void Main(string[] args) => BuildAvaloniaApp()    // Method to start the application, it builds the Avalonia app and starts it with a classic desktop lifetime, passing the command-line arguments to it
        .StartWithClassicDesktopLifetime(args);
    
    public static AppBuilder BuildAvaloniaApp() // Method to configure the Avalonia application, it sets up the application to use platform detection, includes the Inter font, and configures logging to trace output
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}