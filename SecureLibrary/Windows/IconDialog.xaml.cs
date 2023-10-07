using Material.Icons;
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
    /// Interaction logic for IconWindow.xaml
    /// </summary>
    public partial class IconDialog : Window
    {
        private static readonly MaterialIconKind[] IconKinds =
            Enum.GetValues<MaterialIconKind>()
                .ToHashSet()
                .OrderBy(i => i.ToString())
                .ToArray();

        public static readonly DependencyProperty SelectedIconProperty = DependencyProperty.Register(
            nameof(SelectedIcon),
            typeof(MaterialIconKind),
            typeof(IconDialog),
            new(default(MaterialIconKind), SelectedIconChanged)
        );
        public static readonly DependencyProperty IconColorProperty = DependencyProperty.Register(
            nameof(IconColor),
            typeof(Color),
            typeof(IconDialog),
            new(Colors.Black)
        );
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(
            nameof(FilterItem),
            typeof(string),
            typeof(IconDialog),
            new(string.Empty, FilterChanged)
        );

        public IconDialog()
        {
            InitializeComponent();

            IconListBox.ItemsSource = IconKinds;
            IconListBox.Items.Filter = FilterItem;
        }

        public MaterialIconKind SelectedIcon
        {
            get => (MaterialIconKind)GetValue(SelectedIconProperty);
            set => SetValue(SelectedIconProperty, value);
        }

        public Color IconColor
        {
            get => (Color)GetValue(IconColorProperty);
            set => SetValue(IconColorProperty, value);
        }

        public string Filter
        {
            get => (string)GetValue(FilterProperty);
            set => SetValue(FilterProperty, value);
        }

        private bool FilterItem(object? value)
        {
            return value?.ToString() is string str &&
                   str.Contains(Filter, StringComparison.InvariantCultureIgnoreCase);
        }

        private static void FilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IconDialog iw)
            {
                iw.IconListBox.Items.Filter = iw.IconListBox.Items.Filter;
            }
        }

        private void ChangeColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog dialog = new()
            {
                Color = IconColor,
            };
            if (dialog.ShowDialog() == true)
            {
                IconColor = dialog.Color;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Confirm();
        }

        private void IconListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Confirm();
        }

        private void IconWindow_PreviewKeyDown(object sender, KeyEventArgs e)
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

        private static void SelectedIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IconDialog dialog)
            {
                dialog.IconListBox.ScrollIntoView(e.NewValue);
            }
        }
    }
}
