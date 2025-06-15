using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AirTicketSalesManagement.ViewModel.Customer
{
    public partial class BookingHistoryDetailViewModel : BaseViewModel
    {
        private readonly CustomerViewModel parent;
        [ObservableProperty]
        private KQLichSuDatVe lichSuDatVe;
        [ObservableProperty]
        private int tongTien;
        [ObservableProperty]
        private ObservableCollection<Ctdv>? ctdvList;
        [ObservableProperty]
        private bool canCancle;
        public BookingHistoryDetailViewModel() { }
        public BookingHistoryDetailViewModel(KQLichSuDatVe lichSuDatVe, CustomerViewModel parent)
        {
            this.LichSuDatVe = lichSuDatVe;
            this.parent = parent;
            LoadData();
        }
        private async Task LoadData()
        {
            CanCancle = true;
            //CtdvList = new ObservableCollection<Ctdv>
            //{
            //    new Ctdv
            //    {
            //        HoTenHk = "Nguyễn Văn A",
            //        GiayToTuyThan = "012345678",
            //        NgaySinh = new DateOnly(1995, 5, 20),
            //        GioiTinh = "Nam",
            //    },
            //    new Ctdv
            //    {
            //        HoTenHk = "Trần Thị B",
            //        GiayToTuyThan = "987654321",
            //        NgaySinh = new DateOnly(2000, 10, 15),
            //        GioiTinh = "Nữ",
            //    },
            //    new Ctdv
            //    {
            //        HoTenHk = "Lê Minh C",
            //        GiayToTuyThan = null, // Không có giấy tờ
            //        NgaySinh = new DateOnly(2015, 8, 12),
            //        GioiTinh = "Nam",
            //        HoTenNguoiGiamHo = "Nguyễn Thị D",
            //    }
            //};
            try
            {
                using (var context = new AirTicketDbContext())
                {
                    var result = (from ctdv in context.Ctdvs
                                  where ctdv.MaDv == LichSuDatVe.MaVe
                                  select new Ctdv
                                  {
                                      MaDv = ctdv.MaDv,
                                      MaCtdv = ctdv.MaCtdv,
                                      HoTenHk = ctdv.HoTenHk,
                                      GioiTinh = ctdv.GioiTinh,
                                      NgaySinh = ctdv.NgaySinh,
                                      Cccd = ctdv.Cccd,
                                      HoTenNguoiGiamHo = ctdv.HoTenNguoiGiamHo,
                                      MaLv = ctdv.MaLv,
                                      GiaVeTt = ctdv.GiaVeTt
                                  }).ToList();
                    CtdvList = new ObservableCollection<Ctdv>(result);
                }
            }
            catch (Exception e)
            {

            }
        }
        [RelayCommand]
        private void GoBack()
        {
            parent.CurrentViewModel = new BookingHistoryViewModel(UserSession.Current.CustomerId, parent);
        }

        [RelayCommand]
        private async Task CancleTicket()
        {
            if (LichSuDatVe.TrangThai == "Đã hủy")
            {
                MessageBox.Show("Vé đã được hủy trước đó.");
                return;
            }
            if (MessageBox.Show("Bạn có chắc chắn muốn hủy vé này không?", "Xác nhận hủy vé", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new AirTicketDbContext())
                    {
                        var booking = await context.Datves.FirstOrDefaultAsync(b => b.MaDv == LichSuDatVe.MaVe);
                        if (booking != null)
                        {
                            booking.TtdatVe = "Đã hủy";
                            await context.SaveChangesAsync();
                            MessageBox.Show("Hủy vé thành công.");
                            parent.CurrentViewModel = new BookingHistoryViewModel(UserSession.Current.CustomerId, parent);
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy vé để hủy.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi hủy vé: {ex.Message}");
                }
            }
        }
    }
}
