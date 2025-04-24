using AirTicketSalesManagement.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AirTicketSalesManagement.Converters
{
    public class TicketSelectionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 &&
                values[0] is HangVe hangVe &&
                values[1] is KQTraCuuChuyenBayMoRong flight)
            {
                return new SelectedFlightAndTicketClass
                {
                    SelectedTicketClass = hangVe,
                    SelectedFlight = flight
                };
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
