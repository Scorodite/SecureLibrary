using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace SecureLibrary.Windows
{
    /// <summary>
    /// Interaction logic for ColorDialog.xaml
    /// </summary>
    public partial class ColorDialog : Window
    {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            nameof(Color),
            typeof(Color),
            typeof(ColorDialog),
            new(Colors.Black)
        );  

        public ColorDialog()
        {
            InitializeComponent();
        }

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Confirm();
        }

        private void ColorDialog_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    Confirm();
                    break;
                case Key.Escape:
                    DialogResult = false;
                    Close();
                    break;
            }
        }

        private void Confirm()
        {
            DialogResult = true;
            Close();
        }
    }
}
