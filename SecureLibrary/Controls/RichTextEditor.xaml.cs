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
    /// Interaction logic for RichTextEditor.xaml
    /// </summary>
    public partial class RichTextEditor : RichTextBox
    {
        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(
            nameof(Zoom),
            typeof(double),
            typeof(RichTextEditor),
            new(1D)
        );

        public static readonly DependencyProperty CurrentFontFamilyProperty = DependencyProperty.Register(
            nameof(CurrentFontFamily),
            typeof(FontFamily),
            typeof(RichTextEditor),
            new FrameworkPropertyMetadata(
                new FontFamily("Arial"),
                new PropertyChangedCallback(OnCurrentFontFamilyPropertyChanged)
            )
        );

        public static readonly DependencyProperty CurrentFontSizeProperty = DependencyProperty.Register(
            nameof(CurrentFontSize),
            typeof(double),
            typeof(RichTextEditor),
            new FrameworkPropertyMetadata(
                16D,
                new PropertyChangedCallback(OnCurrentFontSizePropertyChanged)
            )
        );

        private bool HandleFontChaged = true;

        public RichTextEditor()
        {
            InitializeComponent();
        }

        public FontFamily CurrentFontFamily
        {
            get => (FontFamily)GetValue(CurrentFontFamilyProperty);
            set => SetValue(CurrentFontFamilyProperty, value);
        }

        public double CurrentFontSize
        {
            get => (double)GetValue(CurrentFontSizeProperty);
            set => SetValue(CurrentFontSizeProperty, value);
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

        private IEnumerable<Paragraph> GetSelectedBlocks()
        {
            Paragraph paragraph = Selection.Start.Paragraph;

            do
            {
                yield return paragraph;
            }
            while (paragraph.NextBlock is Paragraph next &&
                   (paragraph = next) != Selection.End.Paragraph.NextBlock);
        }

        protected override void OnSelectionChanged(RoutedEventArgs e)
        {
            base.OnSelectionChanged(e);

            HandleFontChaged = false;
            if (Selection.GetPropertyValue(FontFamilyProperty) is FontFamily family) CurrentFontFamily = family;
            if (Selection.GetPropertyValue(FontSizeProperty) is double size) CurrentFontSize = size;
            HandleFontChaged = true;
        }

        private void IncreaseFontSizeWrapper(object? parameter)
        {
            EditingCommands.IncreaseFontSize.Execute(parameter, this);
            HandleFontChaged = false;
            CurrentFontSize = (double)Selection.GetPropertyValue(FontSizeProperty);
            HandleFontChaged = true;
        }

        private void DecreaseFontSizeWrapper(object? parameter)
        {
            EditingCommands.DecreaseFontSize.Execute(parameter, this);
            HandleFontChaged = false;
            CurrentFontSize = (double)Selection.GetPropertyValue(FontSizeProperty);
            HandleFontChaged = true;
        }

        #region Property changed handlers

        private static void OnCurrentFontFamilyPropertyChanged(DependencyObject o,
                                                               DependencyPropertyChangedEventArgs e)
        {
            if (o is RichTextEditor editor)
            {
                editor.OnCurrentFontFamilyChanged();
            }
        }

        private static void OnCurrentFontSizePropertyChanged(DependencyObject o,
                                                             DependencyPropertyChangedEventArgs e)
        {
            if (o is RichTextEditor editor)
            {
                editor.OnCurrentFontSizeChanged();
            }
        }

        private void OnCurrentFontFamilyChanged()
        {
            if (HandleFontChaged)
            {
                Selection.ApplyPropertyValue(FontFamilyProperty, CurrentFontFamily);
            }
        }

        private void OnCurrentFontSizeChanged()
        {
            if (HandleFontChaged)
            {
                Selection.ApplyPropertyValue(FontSizeProperty, CurrentFontSize);
            }
        }

        #endregion
    }
}
