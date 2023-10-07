using Material.Icons;
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
    public class BooleanToIconKindConverter : MarkupExtension, IValueConverter
    {
        public BooleanToIconKindConverter(MaterialIconKind trueValue, MaterialIconKind falseValue)
        {
            TrueValue = trueValue;
            FalseValue = falseValue;
        }

        public MaterialIconKind TrueValue { get; }
        public MaterialIconKind FalseValue { get; }

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
                _ => throw new InvalidOperationException("BooleanToIconKindConverter requires bool value"),
            };
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                MaterialIconKind v when v == TrueValue || v == FalseValue => v == TrueValue,
                MaterialIconKind => Binding.DoNothing,
                null => null,
                _ => throw new InvalidOperationException("BooleanToIconKindConverter requires MaterialIconKind value"),
            };
        }
    }
}
