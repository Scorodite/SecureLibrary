using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace SecureLibrary.Utilities.Converters
{
    public class EmptyEnumerableChecker : MarkupExtension, IValueConverter
    {
        public EmptyEnumerableChecker(bool value)
        {
            Value = value;
        }

        public bool Value { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                IEnumerable e => e.Cast<object>().Any() ^ Value,
                null => null,
                _ => throw new InvalidOperationException("EmptyCollectionChecker requires IEnumerable value"),
            };
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("EmptyCollectionChecker can not convert value back");
        }
    }
}
