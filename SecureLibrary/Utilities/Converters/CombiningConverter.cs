using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace SecureLibrary.Utilities.Converters
{
    /// <summary>
    /// Combines two or more converters in single converter
    /// </summary>
    public class CombiningConverter : MarkupExtension, IValueConverter
    {
        public CombiningConverter(IValueConverter[] converters)
        {
            Converters = converters;
        }

        public CombiningConverter(IValueConverter c1, IValueConverter c2)
        {
            Converters = new[] { c1, c2 };
        }

        public CombiningConverter(IValueConverter c1, IValueConverter c2, IValueConverter c3)
        {
            Converters = new[] { c1, c2, c3 };
        }

        public CombiningConverter(IValueConverter c1, IValueConverter c2, IValueConverter c3,
                                  IValueConverter c4)
        {
            Converters = new[] { c1, c2, c3, c4 };
        }

        public IValueConverter[] Converters { get; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object current = value;

            for (int i = 0; i < Converters.Length; i++)
            {
                current = Converters[i].Convert(current, targetType, parameter, culture);
            }

            return current;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object current = value;

            for (int i = Converters.Length - 1; i > -1; i--)
            {
                current = Converters[i].Convert(current, targetType, parameter, culture);
            }

            return current;
        }
    }
}
