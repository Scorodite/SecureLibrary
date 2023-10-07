using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace SecureLibrary.Utilities.Converters
{
    public class ColorToHexConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                Color color => "#" + color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2"),
                null => null,
                _ => throw new InvalidOperationException($"ColorToHexConverter requires Color value")
            };
        }

        public object? ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return Colors.Transparent;
            }
            if (value is string hex)
            {
                Match m = Regex.Match(hex, "^#[A-Fa-f0-9]{6}$");
                if (m.Success)
                {
                    return (Color)ColorConverter.ConvertFromString(m.Value);
                }
                else
                {
                    return DependencyProperty.UnsetValue;
                }
            }

            throw new InvalidOperationException($"ColorToHexConverter requires string value");
        }
    }
}
