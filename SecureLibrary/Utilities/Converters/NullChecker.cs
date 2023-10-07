using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace SecureLibrary.Utilities.Converters
{
    public class NullChecker : MarkupExtension, IValueConverter
    {
        public NullChecker(bool value)
        {
            Value = value;
        }

        public bool Value { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is null ^ Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("NullChecker can not convert value back");
        }
    }
}
