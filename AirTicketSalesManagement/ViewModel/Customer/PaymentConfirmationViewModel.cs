using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AirTicketSalesManagement.ViewModel.Customer
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
                // 1. Tạo mã đặt vé mới (ví dụ random, hoặc tự tăng tùy bạn)
                string maDatVe = GenerateMaDatVe(); // Bạn tự viết hàm này nhé

                // 2. Tạo đối tượng DATVE
                var datVe = new Datve
                {
                    MaDv = maDatVe,
                    MaLb = ThongTinHanhKhachVaChuyenBay.FlightInfo.Flight.MaLichBay,
                    MaKh = "KH001", // giả sử bạn đã có mã khách hàng hiện tại
                    ThoiGianDv = DateTime.Now,
                    Slve = ThongTinHanhKhachVaChuyenBay.FlightInfo.Flight.NumberAdults +
                           ThongTinHanhKhachVaChuyenBay.FlightInfo.Flight.NumberChildren +
                           ThongTinHanhKhachVaChuyenBay.FlightInfo.Flight.NumberInfants,
                    SoDtlienLac = ThongTinHanhKhachVaChuyenBay.ContactPhone,
                    Email = ThongTinHanhKhachVaChuyenBay.ContactEmail,
                    TongTienTt = TotalPrice, // bạn tự viết hàm tính tổng tiền
                    TtdatVe = "Đã thanh toán"
                };

                // Thêm vào DbSet
                context.Datves.Add(datVe);

                // 3. Insert danh sách CTDV
                foreach (var passenger in ThongTinHanhKhachVaChuyenBay.PassengerList)
                {
                    string maCTDV = GenerateMaCTDV(); // tự sinh mã chi tiết đặt vé

                    var ctdv = new Ctdv
                    {
                        MaCtdv = maCTDV,
                        MaDv = maDatVe,
                        HoTenHk = passenger.HoTen,
                        GioiTinh = passenger.GioiTinh,
                        NgaySinh = DateOnly.FromDateTime(passenger.NgaySinh),
                        GiayToTuyThan = passenger.CCCD,
                        HoTenNguoiGiamHo = passenger.HoTenNguoiGiamHo,
                        MaLv = ThongTinHanhKhachVaChuyenBay.FlightInfo.TicketClass.MaHangVe, // mã loại vé
                        GiaVeTt = ThongTinHanhKhachVaChuyenBay.FlightInfo.TicketClass.GiaVe // giá vé
                    };

                    context.Ctdvs.Add(ctdv);
                }

                // 4. Save changes vào database
                context.SaveChanges();
            }
            MessageBox.Show("Đặt vé thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            NavigationService.NavigateTo<HomePageViewModel>();
        }

        private string GenerateMaDatVe()
        {
            return "DV" + DateTime.Now.Ticks.ToString().Substring(10);
        }

        private string GenerateMaCTDV()
        {
            return "CT" + DateTime.Now.Ticks.ToString().Substring(10);
        }
    }
}
