using System.Globalization;

namespace Walkies.MAUI.Utilities
{
    /// <summary>
    /// Converts a string to a boolean by comparing it to the converter
    /// parameter. Used to show or hide UI elements basedon status strings
    /// </summary>
    public class StringEqualsConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string stringValue && parameter is string param)
                return stringValue == param;
            return false;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}