using PresetEditor.Models;
using System.Globalization;

namespace PresetEditor.Converters
{
    internal class InstanceInputNameConveter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not IEnumerable<InputItem> items)
                return string.Empty;

            if (parameter is not string key)
                return string.Empty;

            var itemValue = items.Where(i => i.Key == key)?.FirstOrDefault()?.Value;
            return itemValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
