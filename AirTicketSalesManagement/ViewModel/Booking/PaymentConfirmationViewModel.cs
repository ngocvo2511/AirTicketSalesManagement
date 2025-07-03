using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Diagnostics;
using System.Windows;

namespace AirTicketSalesManagement.ViewModel.Booking
{
    public partial class PaymentConfirmationViewModel : BaseViewModel
    {
        private readonly VnpayPayment vnpayPayment;
        private readonly NotificationViewModel notification;

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

        public NotificationViewModel Notification => notification;

        public PaymentConfirmationViewModel()
        {
            notification = new NotificationViewModel();
        }

        public PaymentConfirmationViewModel(ThongTinHanhKhachVaChuyenBay thongTinHanhKhachVaChuyenBay)
        {
            notification = new NotificationViewModel();
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
                long id = DateTime.Now.Ticks;
                string orderInfo = $"Thanhtoanvemaybay{id}";

                // Tạo URL thanh toán VNPay
                string paymentUrl = vnpayPayment.CreatePaymentUrl((double)TotalPrice, orderInfo, id);

                if (!string.IsNullOrEmpty(paymentUrl))
                {
                    // Lưu thông tin đặt vé tạm thời với trạng thái "Chờ thanh toán"
                    SaveBookingWithPendingStatus("Online");
                    WeakReferenceMessenger.Default.Send(new PaymentRequestedMessage(paymentUrl));

                }
            }
            catch (Exception ex)
            {
                await notification.ShowNotificationAsync(
                    $"Lỗi xử lý thanh toán VNPay: {ex.Message}",
                    NotificationType.Error);
            }
        }


        private void SaveBookingWithPendingStatus(string paymentType)
        {
            int idDatVe = thongTinChuyenBayDuocChon.Id;

            using (var context = new AirTicketDbContext())
            {
                var datVe = context.Datves.FirstOrDefault(dv => dv.MaDv == idDatVe);
                if (datVe == null)
                    return; // hoặc xử lý lỗi

                // Cập nhật thông tin liên lạc
                datVe.SoDtlienLac = ThongTinHanhKhachVaChuyenBay.ContactPhone;
                datVe.Email = ThongTinHanhKhachVaChuyenBay.ContactEmail;
                datVe.TongTienTt = TotalPrice;
                datVe.TtdatVe = $"Chưa thanh toán ({paymentType})"; // chuyển trạng thái
                datVe.ThoiGianDv = DateTime.Now; // cập nhật lại thời gian giữ chỗ

                context.SaveChanges();

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
            }
        }


        private async Task ProcessCashPayment()
        {
            try
            {
                SaveBookingWithPendingStatus("Tiền mặt");
                await notification.ShowNotificationAsync(
                    "Đặt vé thành công! Vui lòng thanh toán tiền mặt tại quầy.",
                    NotificationType.Information);

                NavigateToHomePage();
            }
            catch (Exception ex)
            {
                await notification.ShowNotificationAsync(
                    $"Lỗi xử lý thanh toán tiền mặt: {ex.Message}",
                    NotificationType.Error);
            }
        }

        private void NavigateToHomePage()
        {
            if (UserSession.Current.isStaff)
                NavigationService.NavigateTo<Staff.HomePageViewModel>();
            else
                NavigationService.NavigateTo<Customer.HomePageViewModel>();
        }

        public void HandlePaymentSuccess()
        {
            try
            {
                using (var context = new AirTicketDbContext())
                {
                    Datve datVe = null;

                    if (!UserSession.Current.isStaff) //khach hang
                    {
                        // Trường hợp khách hàng
                        int customerId = UserSession.Current.CustomerId.Value;
                        datVe = context.Datves
                            .Where(dv => dv.MaKh == customerId &&
                                         dv.ThoiGianDv >= DateTime.Now.AddMinutes(-20))
                            .OrderByDescending(dv => dv.ThoiGianDv)
                            .FirstOrDefault();
                    }
                    else
                    {
                        // Trường hợp nhân viên
                        int employeeId = UserSession.Current.StaffId.Value;
                        datVe = context.Datves
                            .Where(dv => dv.MaNv == employeeId &&
                                         dv.ThoiGianDv >= DateTime.Now.AddMinutes(-20))
                            .OrderByDescending(dv => dv.ThoiGianDv)
                            .FirstOrDefault();
                    }

                    if (datVe != null && datVe.TtdatVe == "Chưa thanh toán (Online)")
                    {
                        datVe.TtdatVe = "Đã thanh toán";
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HandlePaymentSuccess] Lỗi cập nhật thanh toán: {ex.Message}");
            }
        }
    }
}