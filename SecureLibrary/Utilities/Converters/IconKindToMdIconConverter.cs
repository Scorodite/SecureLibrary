using Material.Icons;
using Material.Icons.WPF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace SecureLibrary.Utilities.Converters
{
    public class IconKindToMdIconConverter : MarkupExtension, IValueConverter
    {
        public IconKindToMdIconConverter() { }

        public IconKindToMdIconConverter(double? size)
        {
            Size = size;
        }

        public double? Size { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                MaterialIconKind kind => new MaterialIcon()
                    {
                        Kind = kind,
                        Width = Size ?? double.NaN,
                        Height = Size ?? double.NaN,
                    },
                null => null,
                _ => throw new InvalidOperationException("IconKindToMdIconConverter requires MaterialIconKind value")
            };
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                MaterialIcon icon => icon.Kind,
                null => null,
                _ => throw new InvalidOperationException("IconKindToMdIconConverter requires MaterialIcon value")
            };
        }
    }
}
