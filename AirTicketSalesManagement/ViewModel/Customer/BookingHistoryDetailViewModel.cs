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
        private readonly NotificationViewModel notification;

        [ObservableProperty]
        private KQLichSuDatVe lichSuDatVe;

        [ObservableProperty]
        private int tongTien;

        [ObservableProperty]
        private ObservableCollection<Ctdv>? ctdvList;

        [ObservableProperty]
        private bool canCancle;

        public NotificationViewModel Notification => notification;

        public BookingHistoryDetailViewModel()
        {
            notification = new NotificationViewModel();
        }

        public BookingHistoryDetailViewModel(KQLichSuDatVe lichSuDatVe, CustomerViewModel parent)
        {
            notification = new NotificationViewModel();
            this.LichSuDatVe = lichSuDatVe;
            this.parent = parent;
            LoadData();
        }

        private async Task LoadData()
        {
            CanCancle = lichSuDatVe.CanCancel;

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
                                      MaHvLb = ctdv.MaHvLb,
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
        private async Task CancelTicket()
        {
            if (LichSuDatVe.TrangThai == "Đã hủy")
            {
                await notification.ShowNotificationAsync("Vé đã được hủy trước đó.", NotificationType.Warning);
                return;
            }
            if (LichSuDatVe.CanCancel == false)
            {
                await notification.ShowNotificationAsync("Vé không thể hủy do đã quá thời gian hủy.", NotificationType.Warning);
                return;
            }

            bool confirmResult = await notification.ShowNotificationAsync(
                "Bạn có chắc chắn muốn hủy vé này không?",
                NotificationType.Warning,
                isConfirmation: true);

            if (confirmResult)
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
                            await notification.ShowNotificationAsync("Hủy vé thành công.", NotificationType.Information);
                            LichSuDatVe.TrangThai = "Đã hủy";
                            OnPropertyChanged(nameof(LichSuDatVe));
                        }
                        else
                        {
                            await notification.ShowNotificationAsync("Không tìm thấy vé để hủy.", NotificationType.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    await notification.ShowNotificationAsync($"Lỗi khi hủy vé: {ex.Message}", NotificationType.Error);
                }
            }
        }
    }
}