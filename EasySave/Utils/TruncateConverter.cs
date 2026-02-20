using System;
using Avalonia.Data.Converters;
using System.Globalization;

namespace EasySave.Utils
{
    /// <summary>
    /// Value converter that truncates a string to a specified maximum length and appends '...' if it exceeds that length.
    /// </summary>
    public class TruncateConverter : IValueConverter
    {
        /// <summary>
        /// The maximum allowed length of the string before truncation.
        /// </summary>
        public int MaxLength { get; set; } = 50;

        /// <summary>
        /// Converts a string value to a truncated version if it exceeds MaxLength.
        /// </summary>
        /// <param name="value">The value to convert (should be a string).</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">Optional parameter (not used).</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The truncated string with '...' if needed, or the original value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && str.Length > MaxLength)
                return str.Substring(0, MaxLength) + "...";
            return value;
        }

        /// <summary>
        /// Not implemented. Throws NotImplementedException if called.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
