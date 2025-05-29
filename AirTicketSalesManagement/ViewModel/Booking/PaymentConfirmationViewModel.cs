using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.Services;
using AirTicketSalesManagement.ViewModel.Customer;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AirTicketSalesManagement.ViewModel.Booking
{
    public partial class PaymentConfirmationViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string flightCode;

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
        }

        [RelayCommand]
        private void Back()
        {
            NavigationService.NavigateBack();
        }

        [RelayCommand]
        private void ProcessPayment()
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
                context.SaveChanges(); // BẮT BUỘC: để MaDv được gán giá trị từ DB

                int maDatVe = datVe.MaDv;

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
                        MaLv = ThongTinHanhKhachVaChuyenBay.FlightInfo.TicketClass.MaHangVe,
                        GiaVeTt = ThongTinHanhKhachVaChuyenBay.FlightInfo.TicketClass.GiaVe
                    };

                    context.Ctdvs.Add(ctdv);
                }

                context.SaveChanges();
            }

            MessageBox.Show("Đặt vé thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            if (UserSession.Current.isStaff)
                NavigationService.NavigateTo<Staff.HomePageViewModel>();
            else
                NavigationService.NavigateTo<Customer.HomePageViewModel>();
        }


    }
}
