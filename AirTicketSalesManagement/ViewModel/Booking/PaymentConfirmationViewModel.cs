using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.Services;
using AirTicketSalesManagement.ViewModel.Customer;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AirTicketSalesManagement.ViewModel.Booking
{
    public partial class PaymentConfirmationViewModel : BaseViewModel
    {
        private readonly VnpayPayment vnpayPayment;

        [ObservableProperty]
        private string flightCode;

        [ObservableProperty]
        private bool isVNPaySelected = true;

        [ObservableProperty]
        private bool isCashSelected = false;

        public HangVe SelectedTicketClass { get; set; }

        public KQTraCuuChuyenBayMoRong Flight { get; set; }

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
        public PaymentConfirmationViewModel()
        {
        }

        public PaymentConfirmationViewModel(ThongTinHanhKhachVaChuyenBay thongTinHanhKhachVaChuyenBay)
        {
            ThongTinHanhKhachVaChuyenBay = thongTinHanhKhachVaChuyenBay;
            ThongTinChuyenBayDuocChon thongTinChuyenBayDuocChon = thongTinHanhKhachVaChuyenBay.FlightInfo;
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
        }

        [RelayCommand]
        private void Back()
        {
            NavigationService.NavigateBack();
        }

        [RelayCommand]
        private void ProcessPayment()
        {
            if (IsVNPaySelected)
            {
                ProcessVNPayPayment();
            }
            else
            {
                ProcessCashPayment();
            }
        }

        private void ProcessVNPayPayment()
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
                    SaveBookingWithPendingStatus();
                    WeakReferenceMessenger.Default.Send(new PaymentRequestedMessage(paymentUrl));

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xử lý thanh toán VNPay: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveBookingWithPendingStatus()
        {
            using (var context = new AirTicketDbContext())
            {
                var datVe = new Datve
                {
                    MaLb = ThongTinHanhKhachVaChuyenBay.FlightInfo.Flight.MaLichBay,
                    MaKh = UserSession.Current.isStaff ? null : UserSession.Current.CustomerId,
                    MaNv = UserSession.Current.isStaff ? UserSession.Current.StaffId : null,
                    ThoiGianDv = DateTime.Now,
                    Slve = ThongTinHanhKhachVaChuyenBay.FlightInfo.Flight.NumberAdults +
                           ThongTinHanhKhachVaChuyenBay.FlightInfo.Flight.NumberChildren +
                           ThongTinHanhKhachVaChuyenBay.FlightInfo.Flight.NumberInfants,
                    SoDtlienLac = ThongTinHanhKhachVaChuyenBay.ContactPhone,
                    Email = ThongTinHanhKhachVaChuyenBay.ContactEmail,
                    TongTienTt = TotalPrice,
                    TtdatVe = "Chờ thanh toán"
                };

                context.Datves.Add(datVe);
                context.SaveChanges();

                SavePassengerDetails(datVe.MaDv, context);
            }
        }

        private void SavePassengerDetails(int maDatVe, AirTicketDbContext context)
        {
            // Lấy đúng MaHV_LB từ DB
            int maHV_LB = context.Hangvetheolichbays
                .Where(hv => hv.MaLb == ThongTinHanhKhachVaChuyenBay.FlightInfo.Flight.MaLichBay &&
                             hv.MaHv == ThongTinHanhKhachVaChuyenBay.FlightInfo.TicketClass.MaHangVe)
                .Select(hv => hv.MaHvLb)
                .First();

            // Trừ số vé còn lại
            var hangVe = context.Hangvetheolichbays.First(h => h.MaHvLb == maHV_LB);
            hangVe.SlveConLai -= ThongTinHanhKhachVaChuyenBay.PassengerList.Count;

            foreach (var passenger in ThongTinHanhKhachVaChuyenBay.PassengerList)
            {
                var ctdv = new Ctdv
                {
                    MaDv = maDatVe,
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

        private void ProcessCashPayment()
        {
            try
            {
                SaveBookingWithCompletedStatus();
                MessageBox.Show("Đặt vé thành công! Vui lòng thanh toán tiền mặt tại quầy trong vòng 24 giờ.",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                NavigateToHomePage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xử lý thanh toán tiền mặt: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateToHomePage()
        {
            if (UserSession.Current.isStaff)
                NavigationService.NavigateTo<Staff.HomePageViewModel>();
            else
                NavigationService.NavigateTo<Customer.HomePageViewModel>();
        }

        private void SaveBookingWithCompletedStatus()
        {
            using (var context = new AirTicketDbContext())
            {
                var datVe = new Datve
                {
                    MaLb = ThongTinHanhKhachVaChuyenBay.FlightInfo.Flight.MaLichBay,
                    MaKh = UserSession.Current.isStaff ? null : UserSession.Current.CustomerId,
                    MaNv = UserSession.Current.isStaff ? UserSession.Current.StaffId : null,
                    ThoiGianDv = DateTime.Now,
                    Slve = ThongTinHanhKhachVaChuyenBay.FlightInfo.Flight.NumberAdults +
                           ThongTinHanhKhachVaChuyenBay.FlightInfo.Flight.NumberChildren +
                           ThongTinHanhKhachVaChuyenBay.FlightInfo.Flight.NumberInfants,
                    SoDtlienLac = ThongTinHanhKhachVaChuyenBay.ContactPhone,
                    Email = ThongTinHanhKhachVaChuyenBay.ContactEmail,
                    TongTienTt = TotalPrice,
                    TtdatVe = "Đã thanh toán"
                };

                context.Datves.Add(datVe);
                context.SaveChanges();

                SavePassengerDetails(datVe.MaDv, context);
            }
        }
    }


}
