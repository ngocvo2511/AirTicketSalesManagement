using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.Services;
using AirTicketSalesManagement.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using Microsoft.Web.WebView2.Core;
using AirTicketSalesManagement.Interface;
using AirTicketSalesManagement.Services.EmailServices;
using Microsoft.EntityFrameworkCore;

namespace AirTicketSalesManagement.ViewModel.Booking
{
    public partial class PaymentConfirmationViewModel : BaseViewModel
    {
        private readonly IEmailService _emailService;
        private readonly EmailTemplateService _templateService;
        private readonly VnpayPayment vnpayPayment;

        [ObservableProperty]
        private string flightCode;

        [ObservableProperty]
        private string logoUrl;

        [ObservableProperty]
        private bool isVNPaySelected = true;

        [ObservableProperty]
        private bool isCashSelected = false;

        public HangVe SelectedTicketClass { get; set; }

        public KQTraCuuChuyenBayMoRong Flight { get; set; }

        public ThongTinChuyenBayDuocChon thongTinChuyenBayDuocChon { get; set; }

        public ThongTinHanhKhachVaChuyenBay ThongTinHanhKhachVaChuyenBay { get; set; }
        public string AdultSummary { get; set; }
        public string ChildSummary { get; set; }
        public string InfantSummary { get; set; }
        public bool HasChildren { get; set; }
        public bool HasInfants { get; set; }
        public decimal AdultTotalPrice { get; set; }
        public decimal ChildTotalPrice { get; set; }
        public decimal InfantTotalPrice { get; set; }
        public decimal TaxAndFees { get; set; }
        public decimal TotalPrice { get; set; }

        [ObservableProperty]
        public string diemDi;

        [ObservableProperty]
        public string diemDen;
        [ObservableProperty]
        public DateTime thoiGian;
        [ObservableProperty]
        public string flightSummary;


        [ObservableProperty]
        private NotificationViewModel notification = new();

        public PaymentConfirmationViewModel()
        {
        }

        public PaymentConfirmationViewModel(ThongTinHanhKhachVaChuyenBay thongTinHanhKhachVaChuyenBay)
        {
            ThongTinHanhKhachVaChuyenBay = thongTinHanhKhachVaChuyenBay;
            thongTinChuyenBayDuocChon = thongTinHanhKhachVaChuyenBay.FlightInfo;
            FlightCode = $"{thongTinChuyenBayDuocChon.Flight.MaSBDi} - {thongTinChuyenBayDuocChon.Flight.MaSBDen} ({thongTinChuyenBayDuocChon.Flight.HangHangKhong})";
            SelectedTicketClass = thongTinChuyenBayDuocChon.TicketClass;
            Flight = thongTinChuyenBayDuocChon.Flight;
            AdultSummary = $"{Flight.NumberAdults} Người lớn";
            ChildSummary = $"{Flight.NumberChildren} Trẻ em";
            InfantSummary = $"{Flight.NumberInfants} Em bé";
            HasChildren = Flight.NumberChildren > 0;
            HasInfants = Flight.NumberInfants > 0;
            AdultTotalPrice = Flight.NumberAdults * SelectedTicketClass.GiaVe;
            ChildTotalPrice = Flight.NumberChildren * SelectedTicketClass.GiaVe;
            InfantTotalPrice = Flight.NumberInfants * SelectedTicketClass.GiaVe;
            TaxAndFees = 0;
            TotalPrice = (Flight.NumberAdults + Flight.NumberChildren + Flight.NumberInfants) * SelectedTicketClass.GiaVe + TaxAndFees;
            vnpayPayment = new VnpayPayment();
            LogoUrl = GetAirlineLogo(Flight.HangHangKhong);

            DiemDi = thongTinHanhKhachVaChuyenBay.FlightInfo.Flight.DiemDi;
            DiemDen = thongTinHanhKhachVaChuyenBay.FlightInfo.Flight.DiemDen;
            thoiGian = thongTinHanhKhachVaChuyenBay.FlightInfo.Flight.NgayDi;
            FlightSummary = $"{DiemDi} đến {DiemDen} - {ThoiGian.ToString("dddd, dd 'tháng' MM, yyyy", new CultureInfo("vi-VN"))}";
        }

        public PaymentConfirmationViewModel(ThongTinHanhKhachVaChuyenBay thongTinHanhKhachVaChuyenBay, IEmailService emailService, EmailTemplateService emailTemplateService)
        {
            _emailService = emailService;
            _templateService = emailTemplateService;
            ThongTinHanhKhachVaChuyenBay = thongTinHanhKhachVaChuyenBay;
            thongTinChuyenBayDuocChon = thongTinHanhKhachVaChuyenBay.FlightInfo;
            FlightCode = $"{thongTinChuyenBayDuocChon.Flight.MaSBDi} - {thongTinChuyenBayDuocChon.Flight.MaSBDen} ({thongTinChuyenBayDuocChon.Flight.HangHangKhong})";
            SelectedTicketClass = thongTinChuyenBayDuocChon.TicketClass;
            Flight = thongTinChuyenBayDuocChon.Flight;
            AdultSummary = $"{Flight.NumberAdults} Người lớn";
            ChildSummary = $"{Flight.NumberChildren} Trẻ em";
            InfantSummary = $"{Flight.NumberInfants} Em bé";
            HasChildren = Flight.NumberChildren > 0;
            HasInfants = Flight.NumberInfants > 0;
            AdultTotalPrice = Flight.NumberAdults * SelectedTicketClass.GiaVe;
            ChildTotalPrice = Flight.NumberChildren * SelectedTicketClass.GiaVe;
            InfantTotalPrice = Flight.NumberInfants * SelectedTicketClass.GiaVe;
            TaxAndFees = 0;
            TotalPrice = (Flight.NumberAdults + Flight.NumberChildren + Flight.NumberInfants) * SelectedTicketClass.GiaVe + TaxAndFees;
            vnpayPayment = new VnpayPayment();
            LogoUrl = GetAirlineLogo(Flight.HangHangKhong);

            DiemDi = thongTinHanhKhachVaChuyenBay.FlightInfo.Flight.DiemDi;
            DiemDen = thongTinHanhKhachVaChuyenBay.FlightInfo.Flight.DiemDen;
            thoiGian = thongTinHanhKhachVaChuyenBay.FlightInfo.Flight.NgayDi;
            FlightSummary = $"{DiemDi} đến {DiemDen} - {ThoiGian.ToString("dddd, dd 'tháng' MM, yyyy", new CultureInfo("vi-VN"))}";
        }

        private string GetAirlineLogo(string airlineName)
        {
            if (string.IsNullOrWhiteSpace(airlineName))
                return "/Resources/Images/default.png";


            if (airlineName == "Vietnam Airlines")
                return "/Resources/Images/vietnamair.png";
            if (airlineName == "Vietjet Air")
                return "/Resources/Images/vietjet.png";
            if (airlineName == "Bamboo Airways")
                return "/Resources/Images/bamboo.jpg";
            if (airlineName == "Vietravel Airlines")
                return "/Resources/Images/vietravel.png";

            return "/Images/default.png";
        }

        [RelayCommand]
        private void Back()
        {
            NavigationService.NavigateBack();
        }

        [RelayCommand]
        private async Task ProcessPayment()
        {
            using (var context = new AirTicketDbContext())
            {
                var datVe = await context.Datves
                            .Include(b => b.MaLbNavigation)
                            .FirstOrDefaultAsync(dv => dv.MaDv == thongTinChuyenBayDuocChon.Id);
                if(datVe != null && datVe.MaLbNavigation != null && datVe.MaLbNavigation.GioDi != null)
                {
                    var quiDinh = context.Quydinhs.FirstOrDefault();
                    int tgDatVe = quiDinh?.TgdatVeChamNhat ?? 1;
                    if (DateTime.Now > datVe.MaLbNavigation.GioDi.Value.AddDays(-tgDatVe))
                    {
                        await Notification.ShowNotificationAsync(
                            "Thời gian đặt vé đã hết hạn. Vui lòng chọn chuyến bay khác.",
                            NotificationType.Error);
                        NavigationService.NavigateTo<Booking.FlightScheduleSearchViewModel>();
                    }
                }                   
            }
            if (IsVNPaySelected)
            {
                await ProcessVNPayPayment();
            }
            else
            {
                await ProcessCashPayment();
            }
        }

        private async Task ProcessVNPayPayment()
        {
            try
            {
                // Debug thông tin user session
                PaymentDebugHelper.LogUserSession();
                PaymentDebugHelper.ValidateUserSessionForPayment();
                
                long id = DateTime.Now.Ticks;
                string orderInfo = $"Thanhtoanvemaybay{id}";

                // Tạo URL thanh toán VNPay
                string paymentUrl = vnpayPayment.CreatePaymentUrl((double)TotalPrice, orderInfo, id);

                if (!string.IsNullOrEmpty(paymentUrl))
                {
                    // Lưu thông tin đặt vé tạm thời với trạng thái "Chờ thanh toán"
                    SaveBookingWithPendingStatus("Online");
                    await Task.Delay(500); // Đợi một chút để đảm bảo lưu thành công
                    WeakReferenceMessenger.Default.Send(new PaymentRequestedMessage(paymentUrl));
                    Debug.WriteLine($"[ProcessVNPayPayment] Payment URL created successfully: {paymentUrl}");
                }
                else
                {
                    Debug.WriteLine("[ProcessVNPayPayment] Failed to create payment URL");
                    await Notification.ShowNotificationAsync(
                        "Không thể tạo URL thanh toán VNPay",
                        NotificationType.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ProcessVNPayPayment] Error: {ex.Message}");
                Debug.WriteLine($"[ProcessVNPayPayment] Stack trace: {ex.StackTrace}");
                await Notification.ShowNotificationAsync(
                    $"Lỗi xử lý thanh toán VNPay: {ex.Message}",
                    NotificationType.Error);
            }
        }


        private void SaveBookingWithPendingStatus(string paymentType)
        {
            int idDatVe = thongTinChuyenBayDuocChon.Id;            
            Debug.WriteLine($"[SaveBookingWithPendingStatus] Saving booking with ID: {idDatVe}, PaymentType: {paymentType}");
            Debug.WriteLine($"[SaveBookingWithPendingStatus] UserSession - isStaff: {UserSession.Current.isStaff}, CustomerId: {UserSession.Current.CustomerId}, StaffId: {UserSession.Current.StaffId}");

            using (var context = new AirTicketDbContext())
            {
                var datVe = context.Datves.FirstOrDefault(dv => dv.MaDv == idDatVe);
                if (datVe == null)
                {
                    Debug.WriteLine($"[SaveBookingWithPendingStatus] Booking not found with ID: {idDatVe}");
                    return; // hoặc xử lý lỗi
                }

                // Cập nhật thông tin liên lạc
                datVe.SoDtlienLac = ThongTinHanhKhachVaChuyenBay.ContactPhone;
                datVe.Email = ThongTinHanhKhachVaChuyenBay.ContactEmail;
                datVe.TongTienTt = TotalPrice;
                datVe.TtdatVe = $"Chưa thanh toán ({paymentType})"; // chuyển trạng thái
                datVe.ThoiGianDv = DateTime.Now; // cập nhật lại thời gian giữ chỗ


                context.SaveChanges();
                Debug.WriteLine($"[SaveBookingWithPendingStatus] Booking saved successfully with status: {datVe.TtdatVe}");

                // Lưu hành khách
                // Lấy đúng MaHV_LB từ DB
                int maHV_LB = context.Hangvetheolichbays
                    .Where(hv => hv.MaHvLb == ThongTinHanhKhachVaChuyenBay.FlightInfo.TicketClass.MaHangVe)
                    .Select(hv => hv.MaHvLb)
                    .FirstOrDefault();


                foreach (var passenger in ThongTinHanhKhachVaChuyenBay.PassengerList)
                {
                    var ctdv = new Ctdv
                    {
                        MaDv = datVe.MaDv,
                        HoTenHk = passenger.HoTen,
                        GioiTinh = passenger.GioiTinh,
                        NgaySinh = DateOnly.FromDateTime(passenger.NgaySinh),
                        Cccd = passenger.CCCD,
                        HoTenNguoiGiamHo = passenger.HoTenNguoiGiamHo,
                        MaHvLb = maHV_LB,
                        GiaVeTt = ThongTinHanhKhachVaChuyenBay.FlightInfo.TicketClass.GiaVe
                    };

                    context.Ctdvs.Add(ctdv);
                }

                context.SaveChanges();
                Debug.WriteLine($"[SaveBookingWithPendingStatus] Passengers saved successfully");               
            }
        }


        private async Task ProcessCashPayment()
        {
            try
            {
                SaveBookingWithPendingStatus("Tiền mặt");

                await Notification.ShowNotificationAsync(
                    "Đặt vé thành công! Vui lòng thanh toán tiền mặt tại quầy.",
                    NotificationType.Information);
                await Task.Delay(100);
                NavigateToHistory();
                _ = Task.Run(async () =>
                {
                    try
                    {
                        using var context = new AirTicketDbContext();
                        var datVe = await context.Datves
                            .Include(b => b.MaLbNavigation)
                            .FirstOrDefaultAsync(dv => dv.MaDv == thongTinChuyenBayDuocChon.Id);

                        if (datVe != null)
                        {
                            string soHieucb = datVe.MaLbNavigation?.SoHieuCb ?? "";
                            DateTime departureTime = datVe.MaLbNavigation?.GioDi ?? DateTime.Now;
                            var emailContent = _templateService.BuildBookingCash(
                                soHieucb, departureTime, DateTime.Now, TotalPrice);

                            await _emailService.SendEmailAsync(
                                datVe.Email ?? UserSession.Current.Email,
                                "Đặt vé thành công", emailContent);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[ProcessCashPayment] Error sending email: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                await Notification.ShowNotificationAsync(
                    $"Lỗi xử lý thanh toán tiền mặt: {ex.Message}",
                    NotificationType.Error);
            }
        }


        private void NavigateToHistory()
        {
            if (UserSession.Current.isStaff)
                NavigationService.NavigateTo<Staff.TicketManagementViewModel>();
            else
                NavigationService.NavigateTo<Customer.BookingHistoryViewModel>();
        }

        public void HandlePaymentSuccess()
        {
            try
            {
                PaymentDebugHelper.LogUserSession();
                PaymentDebugHelper.ValidateUserSessionForPayment();
                
                using (var context = new AirTicketDbContext())
                {
                    Datve datVe;

                    // Kiểm tra UserSession có hợp lệ không
                    if (UserSession.Current == null)
                    {
                        Debug.WriteLine("[HandlePaymentSuccess] UserSession.Current is null");
                        return;
                    }
                    var searchWindow = DateTime.Now.AddMinutes(-20);
                    Debug.WriteLine($"[HandlePaymentSuccess] Searching from = {searchWindow}, now = {DateTime.Now}");

                    Debug.WriteLine($"[HandlePaymentSuccess] UserSession - isStaff: {UserSession.Current.isStaff}, CustomerId: {UserSession.Current.CustomerId}, StaffId: {UserSession.Current.StaffId}");

                    if (!UserSession.Current.isStaff) //khach hang
                    {
                        // Trường hợp khách hàng
                        if (!UserSession.Current.CustomerId.HasValue)
                        {
                            Debug.WriteLine("[HandlePaymentSuccess] CustomerId is null for customer");
                            return;
                        }
                        
                        int customerId = UserSession.Current.CustomerId.Value;
                        PaymentDebugHelper.LogRecentBookings(customerId: customerId);
                        
                        datVe = context.Datves.Include(dv => dv.MaLbNavigation)
                            .Where(dv => dv.MaKh == customerId &&
                                         dv.ThoiGianDv >= DateTime.Now.AddMinutes(-20))
                            .OrderByDescending(dv => dv.ThoiGianDv)
                            .FirstOrDefault();
                        
                        Debug.WriteLine($"[HandlePaymentSuccess] Found booking for customer {customerId}: {datVe?.MaDv}");
                    }
                    else
                    {
                        // Trường hợp nhân viên
                        if (!UserSession.Current.StaffId.HasValue)
                        {
                            Debug.WriteLine("[HandlePaymentSuccess] StaffId is null for staff");
                            return;
                        }
                        
                        int employeeId = UserSession.Current.StaffId.Value;
                        PaymentDebugHelper.LogRecentBookings(staffId: employeeId);
                        
                        datVe = context.Datves.Include(dv => dv.MaLbNavigation)
                            .Where(dv => dv.MaNv == employeeId &&
                                         dv.ThoiGianDv >= DateTime.Now.AddMinutes(-20))
                            .OrderByDescending(dv => dv.ThoiGianDv)
                            .FirstOrDefault();
                        
                        Debug.WriteLine($"[HandlePaymentSuccess] Found booking for staff {employeeId}: {datVe?.MaDv}");
                    }

                    if (datVe != null)
                    {
                        PaymentDebugHelper.LogPaymentStatus(datVe.MaDv);
                        
                        if (datVe.TtdatVe == "Chưa thanh toán (Online)")
                        {
                            datVe.TtdatVe = "Đã thanh toán";
                            context.SaveChanges();
                            Debug.WriteLine($"[HandlePaymentSuccess] Successfully updated booking {datVe.MaDv} to 'Đã thanh toán'");
                            string soHieuCb = datVe.MaLbNavigation?.SoHieuCb ?? "";
                            var emailContent = _templateService.BuildBookingSuccess(
                                soHieuCb,
                                datVe.MaLbNavigation?.GioDi ?? DateTime.Now,
                                DateTime.Now,
                                datVe.TongTienTt ?? 0);
                            _emailService.SendEmailAsync(datVe.Email ?? UserSession.Current.Email, $"Thanh toán vé chuyến bay {soHieuCb}", emailContent);
                        }
                        else
                        {
                            Debug.WriteLine($"[HandlePaymentSuccess] Booking {datVe.MaDv} status is not 'Chưa thanh toán (Online)'. Current status: {datVe.TtdatVe}");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("[HandlePaymentSuccess] No valid booking found in the last 20 minutes");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HandlePaymentSuccess] Lỗi cập nhật thanh toán: {ex.Message}");
                Debug.WriteLine($"[HandlePaymentSuccess] Stack trace: {ex.StackTrace}");
            }
        }
    }
}