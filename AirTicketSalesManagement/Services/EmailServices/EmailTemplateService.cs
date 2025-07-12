using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Services.EmailServices
{
    public class EmailTemplateService
    {
        
        public string BuildBookingCash(string soHieuCB, DateTime departureTime, DateTime time, decimal price)
        {
            var culture = CultureInfo.GetCultureInfo("vi-VN");
            return $@"
            <p>Bạn đã đặt vé bằng phương thức trả tiền mặt cho chuyến bay <strong>{soHieuCB}</strong>(xuất phát lúc {departureTime:HH:mm dd/MM/yyyy}) lúc {time:HH:mm dd/MM/yyyy}.</p>
            <p>Giá vé là: <strong>{price:C}</strong></p>    
            <p>Vui lòng đến đại lí gần nhất của chúng tôi để thanh toán.</p>
            <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</p>";
        }
        public string BuildBookingSuccess(string soHieuCB, DateTime departureTime, DateTime time, decimal price)
        {
            var culture = CultureInfo.GetCultureInfo("vi-VN");
            return $@"
            <p>Bạn đã đặt vé thành công cho chuyến bay <strong>{soHieuCB}</strong> (xuất phát lúc {departureTime:HH:mm dd/MM/yyyy}) lúc {time:HH:mm dd/MM/yyyy}.</p>
            <p>Giá vé là: <strong>{price.ToString("C", culture)}</strong></p>
            <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</p>";
        }

        public string BuildBookingCancel(string soHieuCB, DateTime departureTime, DateTime time)
            => $@"
            <p>Vé đặt chuyến bay <strong>{soHieuCB}</strong>(xuất phát lúc {departureTime:HH:mm dd/MM/yyyy}) của bạn đã được <strong>hủy</strong> thành công vào lúc {time:HH:mm dd/MM/yyyy}.</p>
            <p>Vui lòng đến đại lí gần nhất của chúng tôi để nhân viên thực hiện quá trình hoàn tiền.</p>
            <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</p>";

        public string BuildForgotPasswordOtp(string otp)
            => $@"
            <p>Bạn đã yêu cầu đặt lại mật khẩu.</p>
            <p>Mã xác nhận của bạn là: <strong>{otp}</strong></p>";

        public string BuildRegisterOtp(string otp)
            => $@"
            <p>Chào mừng bạn đến với hệ thống!</p>
            <p>Mã OTP xác minh đăng ký tài khoản của bạn là: <strong>{otp}</strong></p>";
    }

}
