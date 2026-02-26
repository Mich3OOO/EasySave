using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace EasySave.Utils;

/// <summary>
/// Utility class for applying custom cursor images to Avalonia controls via an attached property.
/// </summary>
public class CursorUtils
{
    /// <summary>
    /// Attached property that defines the image path for a custom cursor.
    /// Can be set in XAML to apply custom cursor images to controls.
    /// </summary>
    public static readonly AttachedProperty<string> ImagePathProperty =
        AvaloniaProperty.RegisterAttached<CursorUtils, Interactive, string>("ImagePath");

    public static void SetImagePath(Interactive element, string value) => element.SetValue(ImagePathProperty, value);

    public static string GetImagePath(Interactive element) => element.GetValue(ImagePathProperty);

    static CursorUtils()
    {
        ImagePathProperty.Changed.AddClassHandler<Interactive>(OnImagePathChanged);
    }

    /// <summary>
    /// Handles changes to the ImagePath attached property.
    /// Loads the specified image, resizes it, and applies it as a custom cursor to the control.
    /// </summary>
    private static void OnImagePathChanged(Interactive control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is string path && !string.IsNullOrEmpty(path))
        {
            try
            {
                var assetUri = new Uri(path);
                using var stream = AssetLoader.Open(assetUri);
                var originalBitmap = new Bitmap(stream);

                var newSize = new PixelSize(64, 64);
                using var resizedBitmap = originalBitmap.CreateScaledBitmap(newSize, BitmapInterpolationMode.HighQuality);

                if (control is Visual visual)
                {
                    if (control is Avalonia.Controls.Control c)
                    {
                        c.Cursor = new Cursor(resizedBitmap, new PixelPoint(0, 0));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CursorHelper Error: {ex.Message}");
            }
        }
    }
}