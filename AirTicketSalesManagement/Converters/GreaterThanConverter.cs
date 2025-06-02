using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AirTicketSalesManagement.Converters
{
    /// <summary>
    /// Converter để kiểm tra giá trị có lớn hơn tham số không
    /// </summary>
    public class GreaterThanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            if (double.TryParse(value.ToString(), out double doubleValue) &&
                double.TryParse(parameter.ToString(), out double threshold))
            {
                return doubleValue > threshold;
            }

            if (int.TryParse(value.ToString(), out int intValue) &&
                int.TryParse(parameter.ToString(), out int intThreshold))
            {
                return intValue > intThreshold;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter để kiểm tra giá trị có nhỏ hơn tham số không
    /// </summary>
    public class LessThanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            if (double.TryParse(value.ToString(), out double doubleValue) &&
                double.TryParse(parameter.ToString(), out double threshold))
            {
                return doubleValue < threshold;
            }

            if (int.TryParse(value.ToString(), out int intValue) &&
                int.TryParse(parameter.ToString(), out int intThreshold))
            {
                return intValue < intThreshold;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter để kiểm tra giá trị có nằm trong khoảng không
    /// Parameter format: "min,max"
    /// </summary>
    public class BetweenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            string paramStr = parameter.ToString();
            string[] parts = paramStr.Split(',');

            if (parts.Length != 2)
                return false;

            if (double.TryParse(value.ToString(), out double doubleValue) &&
                double.TryParse(parts[0].Trim(), out double min) &&
                double.TryParse(parts[1].Trim(), out double max))
            {
                return doubleValue >= min && doubleValue <= max;
            }

            if (int.TryParse(value.ToString(), out int intValue) &&
                int.TryParse(parts[0].Trim(), out int intMin) &&
                int.TryParse(parts[1].Trim(), out int intMax))
            {
                return intValue >= intMin && intValue <= intMax;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NullToDefaultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return parameter ?? "N/A";
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter để format thời gian bay (tính từ giờ đi đến giờ đến)
    /// </summary>
    public class FlightDurationConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return "N/A";

            if (values[0] is DateTime gioDi && values[1] is DateTime gioDen)
            {
                TimeSpan duration = gioDen - gioDi;

                if (duration.TotalMinutes < 0)
                    return "N/A";

                int hours = (int)duration.TotalHours;
                int minutes = duration.Minutes;

                if (hours > 0 && minutes > 0)
                    return $"{hours}h {minutes}m";
                else if (hours > 0)
                    return $"{hours}h";
                else
                    return $"{minutes}m";
            }

            return "N/A";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter để hiển thị trạng thái màu sắc cho số lượng vé
    /// </summary>
    public class SeatAvailabilityColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "#6B7280"; // Gray

            if (int.TryParse(value.ToString(), out int seats))
            {
                if (seats > 20)
                    return "#16A34A"; // Green - Nhiều chỗ
                else if (seats >= 5)
                    return "#D97706"; // Orange - Ít chỗ
                else
                    return "#DC2626"; // Red - Rất ít chỗ
            }

            return "#6B7280"; // Gray - Mặc định
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter để hiển thị icon trạng thái lịch bay
    /// </summary>
    public class FlightStatusIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value?.ToString() ?? string.Empty;

            return status switch
            {
                "Chờ cất cánh" => "ClockOutline",
                "Đã cất cánh" => "AirplaneTakeoff",
                "Hoàn thành" => "CheckCircle",
                _ => "Help"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter để format giá tiền VNĐ
    /// </summary>
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "0 VNĐ";

            if (decimal.TryParse(value.ToString(), out decimal price))
            {
                return price.ToString("N0", culture) + " VNĐ";
            }

            if (double.TryParse(value.ToString(), out double doublePrice))
            {
                return doublePrice.ToString("N0", culture) + " VNĐ";
            }

            return value.ToString() + " VNĐ";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value?.ToString() ?? string.Empty;
            strValue = strValue.Replace(" VNĐ", "").Replace(",", "");

            if (decimal.TryParse(strValue, out decimal result))
                return result;

            return 0;
        }
    }

    /// <summary>
    /// Converter để kiểm tra collection có rỗng không
    /// </summary>
    public class CollectionEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Collections.ICollection collection)
            {
                return collection.Count == 0;
            }

            if (value is System.Collections.IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    return false; // Có ít nhất 1 item
                }
                return true; // Rỗng
            }

            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
