using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.ViewModel.Admin;
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

namespace AirTicketSalesManagement.View.Admin
{
    /// <summary>
    /// Interaction logic for ScheduleManagementView.xaml
    /// </summary>
    public partial class ScheduleManagementView : UserControl
    {

        public ScheduleManagementView()
        {
            InitializeComponent();
        }

        private void DataGridRow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row && row.IsSelected)
            {
                dgLichBay.SelectedItem = null;
                e.Handled = true;
            }
        }

        private void EditButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is Lichbay lichbay)
            {
                var vm = dgLichBay.DataContext as ScheduleManagementViewModel;
                if (vm != null)
                {
                    vm.EditSchedule(lichbay);
                }
            }
            e.Handled = true;
        }

        private void DeleteButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is Lichbay lichbay)
            {
                var vm = dgLichBay.DataContext as ScheduleManagementViewModel;
                if (vm != null)
                {
                    vm.DeleteSchedule(lichbay);
                }
            }
            e.Handled = true;
        }

        private void TimeTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (e.Key == Key.Back)
            {
                string text = textBox.Text;
                int caretIndex = textBox.CaretIndex;

                // Nếu cursor ở sau dấu : (ví dụ: "12:|" - cursor ở vị trí 3)
                if (caretIndex == 3 && text.Length >= 3 && text.Substring(0, 3).EndsWith(":"))
                {
                    // Xóa cả dấu : và số trước đó
                    textBox.Text = text.Substring(0, 1) + (text.Length > 3 ? text.Substring(3) : "");
                    textBox.CaretIndex = 1;
                    e.Handled = true;
                }
                // Nếu cursor ở trước dấu : và sắp xóa nó
                else if (caretIndex > 0 && caretIndex < text.Length && text[caretIndex - 1] == ':')
                {
                    // Xóa dấu : và ký tự trước đó
                    string beforeColon = text.Substring(0, caretIndex - 2);
                    string afterColon = text.Substring(caretIndex);
                    textBox.Text = beforeColon + afterColon;
                    textBox.CaretIndex = Math.Max(0, caretIndex - 2);
                    e.Handled = true;
                }
            }
        }

        private void TimeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            // Chỉ cho phép số (dấu : sẽ được tự động thêm)
            if (!char.IsDigit(e.Text[0]))
            {
                e.Handled = true;
                return;
            }
            // Kiểm tra độ dài tối đa (4 số: HHMM)
            string currentDigits = textBox.Text.Replace(":", "");
            if (currentDigits.Length >= 4)
            {
                e.Handled = true;
            }
        }

        private void TimeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string text = textBox.Text;

            // Chỉ xử lý khi user nhập, không phải do code set
            if (!textBox.IsFocused) return;

            // Auto format: thêm dấu : sau 2 số đầu
            if (text.Length == 2 && !text.Contains(":") &&
                text.All(char.IsDigit))
            {
                textBox.Text = text + ":";
                textBox.CaretIndex = textBox.Text.Length;
            }
        }

        private void TimeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string timeText = textBox.Text.Trim();

            // Nếu rỗng thì bỏ qua
            if (string.IsNullOrEmpty(timeText))
            {
                ClearError(textBox);
                return;
            }
                

            if (!IsValidTimeFormat(timeText))
            {
                // Hiển thị lỗi
                ShowTimeError(textBox, "Thời gian không hợp lệ");
            }
            else
            {
                // Xóa lỗi nếu có
                ClearError(textBox);
            }
        }

        private bool IsValidTimeFormat(string timeText)
        {
            // Kiểm tra format HH:MM
            if (!System.Text.RegularExpressions.Regex.IsMatch(timeText, @"^\d{2}:\d{2}$"))
                return false;

            try
            {
                string[] parts = timeText.Split(':');
                int hour = int.Parse(parts[0]);
                int minute = int.Parse(parts[1]);

                // Kiểm tra giờ hợp lệ (0-23)
                if (hour < 0 || hour > 23)
                    return false;

                // Kiểm tra phút hợp lệ (0-59)
                if (minute < 0 || minute > 59)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ShowTimeError(TextBox textBox, string errorMessage)
        {
            if (textBox.Tag is Label label)
            {
                label.Content = errorMessage;
                label.Visibility = Visibility.Visible;
            }
            textBox.BorderBrush = new SolidColorBrush(Colors.Red);
        }

        private void ClearError(TextBox textBox)
        {
            if (textBox.Tag is Label label)
            {
                label.Visibility = Visibility.Collapsed;
            }
            textBox.ClearValue(TextBox.BorderBrushProperty);
        }

    }

}
