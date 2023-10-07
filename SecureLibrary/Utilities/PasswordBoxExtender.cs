using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace SecureLibrary.Utilities
{
    public static class PasswordBoxExtender
    {
        #region Dependency Properties

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached(
            "Password",
            typeof(string), 
            typeof(PasswordBoxExtender),
            new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged)
        );

        public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached(
            "Attach",
            typeof(bool),
            typeof(PasswordBoxExtender),
            new PropertyMetadata(false, OnAttachPropertyChanged)
        );

        private static readonly DependencyProperty IsUpdatingProperty = DependencyProperty.RegisterAttached(
            "IsUpdating",
            typeof(bool),
            typeof(PasswordBoxExtender)
        );

        #endregion

        #region Setters/Getters

        public static void SetAttach(this DependencyObject o, bool value)
        {
            o.SetValue(AttachProperty, value);
        }

        public static void SetPassword(this DependencyObject o, string value)
        {
            o.SetValue(PasswordProperty, value);
        }

        private static void SetIsUpdating(this DependencyObject o, bool value)
        {
            o.SetValue(IsUpdatingProperty, value);
        }

        public static bool GetAttach(this DependencyObject o)
        {
            return (bool)o.GetValue(AttachProperty);
        }

        public static string GetPassword(this DependencyObject o)
        {
            return (string)o.GetValue(PasswordProperty);
        }

        private static bool GetIsUpdating(this DependencyObject o)
        {
            return (bool)o.GetValue(IsUpdatingProperty);
        }

        #endregion

        #region Property changed handers

        private static void OnPasswordPropertyChanged(DependencyObject sender,
                                                      DependencyPropertyChangedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;

                if (!GetIsUpdating(passwordBox))
                {
                    passwordBox.Password = (string)e.NewValue;
                }

                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
            }
        }

        private static void OnAttachPropertyChanged(DependencyObject sender,
                                                    DependencyPropertyChangedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                if ((bool)e.OldValue)
                {
                    passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                }

                if ((bool)e.NewValue)
                {
                    passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
                }
            }

        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                passwordBox.SetIsUpdating(true);
                passwordBox.SetPassword(passwordBox.Password);
                passwordBox.SetIsUpdating(false);
            }
        }

        #endregion
    }
}
