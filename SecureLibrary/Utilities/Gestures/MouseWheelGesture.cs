using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SecureLibrary.Utilities.Gestures
{
    public class MouseWheelGesture : MouseGesture
    {
        public MouseWheelGesture() : base(MouseAction.WheelClick) { }

        public MouseWheelGesture(ModifierKeys modifiers) : base(MouseAction.WheelClick, modifiers) { }

        public MouseWheelGesture(ModifierKeys modifiers, WheelDirection direction) : this(modifiers)
        {
            Direction = direction;
        }

        public WheelDirection Direction { get; set; }

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs) =>
            base.Matches(targetElement, inputEventArgs) &&
            inputEventArgs is MouseWheelEventArgs args &&
            Direction switch
            {
                WheelDirection.None => args.Delta == 0,
                WheelDirection.Up => args.Delta > 0,
                WheelDirection.Down => args.Delta < 0,
                _ => false,
            };

        public enum WheelDirection
        {
            None,
            Up,
            Down,
        }
    }
}
