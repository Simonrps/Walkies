using System.Globalization;

namespace Walkies.MAUI.Utilities
{
    /// <summary>
    /// Converts a boolean validation state to a Color calue for use as a border color on form entry fields.
    /// </summary>
    public class BoolToValidationColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts boolean validation state to a Color
        /// </summary>
        /// <param name="value">the boolean validation state</param>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool isValid && isValid ? Colors.LightGray : Colors.Red;
        }

        /// <summary>
        /// Not implemented. Converts a Color back to a boolean validation state.
        /// </summary>
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}