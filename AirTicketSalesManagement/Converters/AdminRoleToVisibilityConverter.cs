using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AirTicketSalesManagement.Converters
{
    public class AdminRoleToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string role)
            {
                return role == "ADMIN" ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 