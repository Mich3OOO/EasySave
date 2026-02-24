using System;
using Avalonia.Data.Converters;
using System.Globalization;

namespace EasySave.Utils
{
    /// <summary>
    /// Value converter that truncates a string to a specified maximum length and
    /// appends '...' if it exceeds that length
    /// </summary>
    public class TruncateConverter : IValueConverter
    {
        public int MaxLength { get; set; } = 50;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && str.Length > MaxLength)
                return str.Substring(0, MaxLength) + "...";
            return value;
        }

        /// <summary>
        /// This method is required by the IValueConverter interface (from Avalonia) even though it is not used in this converter.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
