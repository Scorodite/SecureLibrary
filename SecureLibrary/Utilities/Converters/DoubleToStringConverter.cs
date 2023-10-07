using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.Windows.Markup;

namespace SecureLibrary.Utilities.Converters
{
    public class DoubleToStringConverter : MarkupExtension, IValueConverter
    {
        public double? Mininum { get; set; }
        public double? Maximum { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                double number => number.ToString(),
                null => null,
                _ => throw new InvalidOperationException("DoubleToStringConverter requires double value"),
            };
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                string str when double.TryParse(str, out var number) =>
                    Maximum is null || number <= Maximum ?
                        Mininum is null || number >= Mininum ?
                            number :
                            Mininum :
                        Maximum,
                string => DependencyProperty.UnsetValue,
                null => null,
                _ => throw new InvalidOperationException("DoubleToStringConverter requires string value"),
            };
        }
    }
}
