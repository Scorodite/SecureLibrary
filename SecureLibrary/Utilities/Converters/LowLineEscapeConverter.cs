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
    /// Required to escape strings for Labels
    /// </summary>
    public class LowLineEscapeConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                string str => str.Replace("_", "__"),
                null => null,
                _ => throw new InvalidOperationException("LowLineEscapeConverter requires string value"),
            };
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                string str => str.Replace("__", "_"),
                null => null,
                _ => throw new InvalidOperationException("LowLineEscapeConverter requires string value"),
            };
        }
    }
}
