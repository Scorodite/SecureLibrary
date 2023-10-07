using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace SecureLibrary.Utilities
{
    public static class ColorExtender
    {
        #region Dependency Properties

        public readonly static DependencyProperty ColorProperty = DependencyProperty.RegisterAttached(
            "Color",
            typeof(Color),
            typeof(ColorExtender),
            new PropertyMetadata(Colors.Black, OnColorPropertyChanged)
        );

        public readonly static DependencyProperty AlphaChannelProperty = DependencyProperty.RegisterAttached(
            "AlphaChannel",
            typeof(byte),
            typeof(ColorExtender),
            new PropertyMetadata(OnAlphaChannelPropertyChanged)
        );

        public readonly static DependencyProperty RedChannelProperty = DependencyProperty.RegisterAttached(
            "RedChannel",
            typeof(byte),
            typeof(ColorExtender),
            new PropertyMetadata(OnRedChannelPropertyChanged)
        );

        public readonly static DependencyProperty GreenChannelProperty = DependencyProperty.RegisterAttached(
            "GreenChannel",
            typeof(byte),
            typeof(ColorExtender),
            new PropertyMetadata(OnGreenChannelPropertyChanged)
        );

        public readonly static DependencyProperty BlueChannelProperty = DependencyProperty.RegisterAttached(
            "BlueChannel",
            typeof(byte),
            typeof(ColorExtender),
            new PropertyMetadata(OnBlueChannelPropertyChanged)
        );

        #endregion

        #region Getters/Setters

        public static void SetRedChannel(this DependencyObject o, byte value)
        {
            o.SetValue(RedChannelProperty, value);
        }

        public static void SetGreenChannel(this DependencyObject o, byte value)
        {
            o.SetValue(GreenChannelProperty, value);
        }

        public static void SetBlueChannel(this DependencyObject o, byte value)
        {
            o.SetValue(BlueChannelProperty, value);
        }

        public static void SetAlphaChannel(this DependencyObject o, byte value)
        {
            o.SetValue(AlphaChannelProperty, value);
        }

        public static void SetColor(this DependencyObject o, Color value)
        {
            o.SetValue(ColorProperty, value);
        }

        public static byte GetRedChannel(this DependencyObject o)
        {
            return (byte)o.GetValue(RedChannelProperty);
        }

        public static byte GetGreenChannel(this DependencyObject o)
        {
            return (byte)o.GetValue(GreenChannelProperty);
        }

        public static byte GetBlueChannel(this DependencyObject o)
        {
            return (byte)o.GetValue(BlueChannelProperty);
        }

        public static Color GetColor(this DependencyObject o)
        {
            return (Color)o.GetValue(ColorProperty);
        }

        #endregion

        #region Property changed handers

        private static void OnAlphaChannelPropertyChanged(DependencyObject d,
                                                          DependencyPropertyChangedEventArgs e)
        {
            Color color = d.GetColor();
            if (color.A == (byte)e.NewValue) return;
            d.SetColor(Color.FromArgb((byte)e.NewValue, color.R, color.G, color.B));
        }

        private static void OnRedChannelPropertyChanged(DependencyObject d, 
                                                        DependencyPropertyChangedEventArgs e)
        {
            Color color = d.GetColor();
            if (color.R == (byte)e.NewValue) return;
            d.SetColor(Color.FromArgb(color.A, (byte)e.NewValue, color.G, color.B));
        }

        private static void OnGreenChannelPropertyChanged(DependencyObject d,
                                                          DependencyPropertyChangedEventArgs e)
        {
            Color color = d.GetColor();
            if (color.G == (byte)e.NewValue) return;
            d.SetColor(Color.FromArgb(color.A, color.R, (byte)e.NewValue, color.B));
        }

        private static void OnBlueChannelPropertyChanged(DependencyObject d,
                                                         DependencyPropertyChangedEventArgs e)
        {
            Color color = d.GetColor();
            if (color.B == (byte)e.NewValue) return;
            d.SetColor(Color.FromArgb(color.A, color.R, color.G, (byte)e.NewValue));
        }


        private static void OnColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Color color = (Color)e.NewValue;
            if (color == (Color)e.OldValue) return;
            d.SetAlphaChannel(color.A);
            d.SetRedChannel(color.R);
            d.SetGreenChannel(color.G);
            d.SetBlueChannel(color.B);
        }

        #endregion
    }
}
