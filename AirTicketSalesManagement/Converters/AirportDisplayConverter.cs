using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AirTicketSalesManagement.Converters
{
    public class AirportDisplayConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string tenSb = values[0] as string;
            string maSb = values[1] as string;

            if (string.IsNullOrWhiteSpace(tenSb) || string.IsNullOrWhiteSpace(maSb))
                return string.Empty;

            return $"{tenSb} ({maSb})";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
