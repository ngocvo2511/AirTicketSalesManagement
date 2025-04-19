using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AirTicketSalesManagement.Helper
{
    public static class PasswordHelper
    {
        public static readonly DependencyProperty BoundPassword =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordHelper), new PropertyMetadata(string.Empty, OnBoundPasswordChanged));
       
        public static string GetBoundPassword(DependencyObject obj)
            => (string)obj.GetValue(BoundPassword);

        public static void SetBoundPassword(DependencyObject obj, string value)
            => obj.SetValue(BoundPassword, value);

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                passwordBox.PasswordChanged -= PasswordChanged;
                if (!(bool)GetIsUpdating(passwordBox))
                    passwordBox.Password = (string)e.NewValue;
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        public static readonly DependencyProperty Attach =
            DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(PasswordHelper), new PropertyMetadata(false, AttachChanged));

        public static bool GetAttach(DependencyObject obj)
            => (bool)obj.GetValue(Attach);

        public static void SetAttach(DependencyObject obj, bool value)
            => obj.SetValue(Attach, value);

        private static void AttachChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                if ((bool)e.NewValue)
                    passwordBox.PasswordChanged += PasswordChanged;
                else
                    passwordBox.PasswordChanged -= PasswordChanged;
            }
        }

        private static readonly DependencyProperty IsUpdating =
            DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(PasswordHelper));

        private static bool GetIsUpdating(DependencyObject obj)
            => (bool)obj.GetValue(IsUpdating);

        private static void SetIsUpdating(DependencyObject obj, bool value)
            => obj.SetValue(IsUpdating, value);

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            SetIsUpdating(passwordBox, true);
            SetBoundPassword(passwordBox, passwordBox.Password);
            SetIsUpdating(passwordBox, false);
        }
    }
}
