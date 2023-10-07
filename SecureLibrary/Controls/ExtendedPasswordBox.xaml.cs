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
    /// Interaction logic for ExtendedPasswordBox.xaml
    /// </summary>
    public partial class ExtendedPasswordBox : TextBox
    {
        public static readonly DependencyProperty IsRevealedProperty = DependencyProperty.Register(
            nameof(IsRevealed),
            typeof(bool),
            typeof(ExtendedPasswordBox),
            new(false, OnRevealedPropertyChanged)
        );

        public event EventHandler? IsRevealedChanged;

        public ExtendedPasswordBox()
        {
            InitializeComponent();
        }

        public bool IsRevealed
        {
            get => (bool)GetValue(IsRevealedProperty);
            set => SetValue(IsRevealedProperty, value);
        }

        private static void OnRevealedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ExtendedPasswordBox box)
            {
                box.IsRevealedChanged?.Invoke(box, new());
            }
        }

        private void IsRevealedSwitch_Click(object sender, RoutedEventArgs e)
        {
            IsRevealed = !IsRevealed;
        }
    }
}
