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

    /// <summary>
    /// Sets the ImagePath attached property value for the specified element.
    /// </summary>
    /// <param name="element">The element to set the cursor image path on.</param>
    /// <param name="value">The path to the cursor image (e.g., avaloniaresource://...).</param>
    public static void SetImagePath(Interactive element, string value) => element.SetValue(ImagePathProperty, value);

    /// <summary>
    /// Gets the ImagePath attached property value for the specified element.
    /// </summary>
    /// <param name="element">The element to get the cursor image path from.</param>
    /// <returns>The path to the cursor image.</returns>
    public static string GetImagePath(Interactive element) => element.GetValue(ImagePathProperty);

    /// <summary>
    /// Static constructor that registers a handler for ImagePath property changes.
    /// </summary>
    static CursorUtils()
    {
        // Listen for property changes to apply the cursor
        ImagePathProperty.Changed.AddClassHandler<Interactive>(OnImagePathChanged);
    }

    /// <summary>
    /// Handles changes to the ImagePath attached property.
    /// Loads the specified image, resizes it, and applies it as a custom cursor to the control.
    /// </summary>
    /// <param name="control">The control whose ImagePath property changed.</param>
    /// <param name="e">Event arguments containing the new property value.</param>
    private static void OnImagePathChanged(Interactive control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is string path && !string.IsNullOrEmpty(path))
        {
            try
            {
                var assetUri = new Uri(path);
                using var stream = AssetLoader.Open(assetUri);
                var originalBitmap = new Bitmap(stream);

                // Resize (e.g., 64x64)
                var newSize = new PixelSize(64, 64);
                using var resizedBitmap = originalBitmap.CreateScaledBitmap(newSize, BitmapInterpolationMode.HighQuality);

                // Apply the cursor to the control (Window, Button, etc.)
                if (control is Visual visual)
                {
                    // Note: In recent versions, Cursor is accessed via the direct property
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