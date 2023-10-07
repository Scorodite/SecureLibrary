using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace SecureLibrary.Utilities.Converters
{
    public class ColorToBrushConverter : MarkupExtension, IValueConverter
    {
        public string? NullValueResource { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                Color color => new SolidColorBrush(color),
                null when NullValueResource is not null => App.Current.TryFindResource(NullValueResource),
                null => null,
                _ => throw new InvalidOperationException($"ColorToBrushConverter requires Color value"),
            };
        }

        public object? ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                SolidColorBrush brush when
                        NullValueResource is not null &&
                        brush == App.Current.TryFindResource(NullValueResource)
                    => Binding.DoNothing,
                SolidColorBrush brush => brush.Color,
                Brush => Binding.DoNothing,
                null => null,
                _ => throw new InvalidOperationException($"ColorToBrushConverter requires Brush value"),
            };
        }
    }
}
