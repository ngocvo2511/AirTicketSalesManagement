using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
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
                                      GiayToTuyThan = ctdv.GiayToTuyThan,
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
            parent.CurrentViewModel = new BookingHistoryViewModel(parent.IdCustomer, parent);
        }
        [RelayCommand]
        private async Task CancelPassenger(Ctdv ctdv)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn huỷ hành khách này?", "Xác nhận huỷ", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                await RemovePassenger(ctdv);
            }
        }
        private async Task RemovePassenger(Ctdv ctdv)
        {
            try
            {
                using (var context = new AirTicketDbContext())
                {
                    var khachHang = context.Ctdvs.FirstOrDefault(v => v.MaCtdv == ctdv.MaCtdv);
                    if (khachHang != null)
                    {
                        context.Ctdvs.Remove(khachHang);
                        await context.SaveChangesAsync();
                        if (CtdvList != null)
                        {
                            CtdvList.Remove(ctdv);
                        }
                        if (CtdvList == null || CtdvList.Count == 0)
                        {
                            var datVe = context.Datves.FirstOrDefault(v => v.MaDv == ctdv.MaDv);
                            if (datVe != null)
                            {
                                context.Datves.Remove(datVe);
                                await context.SaveChangesAsync();
                            }
                            GoBack();
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }
        [RelayCommand]
        private async Task CancelAllPassenger()
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn huỷ tất cả hành khách?", "Xác nhận huỷ", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                await RemoveAllPassenger();
            }
        }
        private async Task RemoveAllPassenger()
        {
            try
            {
                using (var context = new AirTicketDbContext())
                {
                    var Ve = context.Datves.FirstOrDefault(v=> v.MaDv == LichSuDatVe.MaVe);
                    if (Ve != null)
                    {
                        context.Datves.Remove(Ve);
                        await context.SaveChangesAsync();
                    }
                    GoBack();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
