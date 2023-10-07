using Material.Icons;
using Material.Icons.WPF;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace SecureLibrary.Utilities.Converters
{
    /// <summary>
    /// Required for content bindings in styles, because they can not
    /// be applied twice using MaterialIcon xaml tag 
    /// </summary>
    public class MdIconGenerator : MarkupExtension, IValueConverter
    {
        public MdIconGenerator(MaterialIconKind icon)
        {
            Icon = icon;
        }

        public MdIconGenerator(MaterialIconKind icon, double? size) : this(icon)
        {
            Size = size;
        }

        public MaterialIconKind Icon { get; }
        public double? Size { get; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new MaterialIcon()
            {
                Kind = Icon,
                Width = Size ?? double.NaN,
                Height = Size ?? double.NaN,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("Can not convert icon back to anything");
        }
    }
}
