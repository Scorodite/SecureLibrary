using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace SecureLibrary.Utilities.Converters
{
    public class BooleanToVisibilityConverter : MarkupExtension, IValueConverter
    {
        public BooleanToVisibilityConverter(Visibility trueValue, Visibility falseValue)
        {
            TrueValue = trueValue;
            FalseValue = falseValue;
        }

        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                bool v => v ? TrueValue : FalseValue,
                null => null,
                _ => throw new InvalidOperationException("BooleanToVisibilityConverter requires bool value"),
            };
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                Visibility v when v == TrueValue || v == FalseValue => v == TrueValue,
                Visibility => DependencyProperty.UnsetValue,
                null => null,
                _ => throw new InvalidOperationException("BooleanToVisibilityConverter requires bool value"),
            };
        }
    }
}
