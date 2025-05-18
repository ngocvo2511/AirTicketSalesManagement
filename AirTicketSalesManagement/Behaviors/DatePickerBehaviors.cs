using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace AirTicketSalesManagement.Behaviors
{
    public static class DatePickerBehaviors
    {
        public static readonly DependencyProperty IsTextInputDisabledProperty =
            DependencyProperty.RegisterAttached(
                "IsTextInputDisabled",
                typeof(bool),
                typeof(DatePickerBehaviors),
                new UIPropertyMetadata(false, OnIsTextInputDisabledChanged));

        public static bool GetIsTextInputDisabled(DependencyObject obj)
            => (bool)obj.GetValue(IsTextInputDisabledProperty);

        public static void SetIsTextInputDisabled(DependencyObject obj, bool value)
            => obj.SetValue(IsTextInputDisabledProperty, value);

        private static void OnIsTextInputDisabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DatePicker datePicker && (bool)e.NewValue)
            {
                datePicker.Loaded += (s, args) =>
                {
                    if (datePicker.Template.FindName("PART_TextBox", datePicker) is DatePickerTextBox textBox)
                    {
                        textBox.IsReadOnly = true;
                        textBox.Cursor = Cursors.Arrow;
                    }
                };
            }
        }
    }
}
