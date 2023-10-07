using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SecureLibrary.Controls
{
    /// <summary>
    /// Interaction logic for ExtendedTextBox.xaml
    /// </summary>
    public partial class ExtendedTextBox : TextBox
    {
        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(
            nameof(Zoom),
            typeof(double),
            typeof(ExtendedTextBox),
            new(1D)
        );

        public ExtendedTextBox()
        {
            InitializeComponent();
        }

        public double Zoom
        {
            get => (double)GetValue(ZoomProperty);
            set => SetValue(ZoomProperty, value);
        }

        private void OnCtrlWheelUp(object? parameter)
        {
            if (Zoom < 10)
            {
                Zoom *= 1.1;
            }
        }

        private void OnCtrlWheelDown(object? parameter)
        {
            if (Zoom > 0.1)
            {
                Zoom /= 1.1;
            }
        }
    }
}
