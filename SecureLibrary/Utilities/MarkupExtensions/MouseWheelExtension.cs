using SecureLibrary.Utilities.Gestures;
using System;
using System.Windows.Input;
using System.Windows.Markup;

namespace SecureLibrary.Utilities.MarkupExtensions
{
    public class MouseWheelExtension : MarkupExtension
    {
        public MouseWheelExtension() { }

        public MouseWheelGesture.WheelDirection Direction { get; set; }
        public ModifierKeys Modifier { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new MouseWheelGesture(Modifier, Direction);
        }
    }
}
