using PresetEditor.Models;
using System.Globalization;

namespace PresetEditor.Converters
{
    internal class InstanceInputNameConveter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is IEnumerable<InputItem> items && parameter is string key)
                return items.FirstOrDefault(i => i.Key == key)?.Value ?? string.Empty;
            return string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
