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
    public class SelectedFlightAndTicketClassConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 &&
                values[0] is HangVe ticketClass &&
                values[1] is KQTraCuuChuyenBayMoRong flight)
            {
                return new ThongTinChuyenBayDuocChon
                {
                    TicketClass = ticketClass,
                    Flight = flight
                };
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
