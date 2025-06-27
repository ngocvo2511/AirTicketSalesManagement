using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.Models.UIModels;
using AirTicketSalesManagement.Services;
using AirTicketSalesManagement.ViewModel.Admin;
using AirTicketSalesManagement.ViewModel.Customer;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AirTicketSalesManagement.ViewModel.Staff
{
    public partial class TicketManagementDetailViewModel : BaseViewModel
    {
        private readonly BaseViewModel parent;
        [ObservableProperty]
        private QuanLiDatVe chiTietVe;
        [ObservableProperty]
        private int tongTien;
        [ObservableProperty]
        private ObservableCollection<Ctdv>? ctdvList;
        [ObservableProperty]
        private bool canCancle;
        public TicketManagementDetailViewModel() { }
        public TicketManagementDetailViewModel(QuanLiDatVe chiTietVe, BaseViewModel parent)
        {
            this.ChiTietVe = chiTietVe;
            this.parent = parent;
            LoadData();
        }
        private async Task LoadData()
        {
            CanCancle = ChiTietVe.CanCancel;

            try
            {
                using (var context = new AirTicketDbContext())
                {
                    var result = await context.Ctdvs
                        .Where(ctdv => ctdv.MaDv == ChiTietVe.MaVe)
                        .Select(ctdv => new Ctdv
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
                        })
                        .ToListAsync();
                    CtdvList = new ObservableCollection<Ctdv>(result);
                }
            }
            catch (Exception e)
            {
                // Handle exception (optional logging or user notification)
            }
        }
        [RelayCommand]
        private void GoBack()
        {
            if(parent is StaffViewModel staffViewModel)
            {
                staffViewModel.CurrentViewModel = new TicketManagementViewModel(parent);
            }
            else if (parent is AdminViewModel adminViewModel)
            {
                adminViewModel.CurrentViewModel = new TicketManagementViewModel(parent);
            }
        }
        [RelayCommand]
        private async Task CancelTicket()
        {
            if (ChiTietVe.TrangThai == "Đã hủy")
            {
                MessageBox.Show("Vé đã được hủy trước đó.");
                return;
            }
            if(ChiTietVe.CanCancel == false)
            {
                MessageBox.Show("Vé không thể hủy do đã quá thời gian hủy.");
                return;
            }
            if (MessageBox.Show("Bạn có chắc chắn muốn hủy vé này không?", "Xác nhận hủy vé", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new AirTicketDbContext())
                    {
                        var booking = await context.Datves.FirstOrDefaultAsync(b => b.MaDv == ChiTietVe.MaVe);
                        if (booking != null)
                        {
                            booking.TtdatVe = "Đã hủy";
                            await context.SaveChangesAsync();
                            MessageBox.Show("Hủy vé thành công.");
                            ChiTietVe.TrangThai = "Đã hủy";
                            OnPropertyChanged(nameof(ChiTietVe));
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
        [RelayCommand]
        private async Task ConfirmPayment()
        {
            if (ChiTietVe.TrangThai != "Chờ thanh toán")
            {
                MessageBox.Show("Không thể xác nhận thanh toán.");
                return;
            }
            if(ChiTietVe.CanConfirm == false)
            {
                MessageBox.Show("Vé không thể thanh toán do đã quá thời gian đặt vé.");
                return;
            }
            if (MessageBox.Show("Bạn có chắc chắn muốn xác nhận thanh toán vé này không?", "Xác nhận thanh toán", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new AirTicketDbContext())
                    {
                        var booking = await context.Datves.FirstOrDefaultAsync(b => b.MaDv == ChiTietVe.MaVe);
                        if (booking != null)
                        {
                            booking.TtdatVe = "Đã thanh toán";
                            await context.SaveChangesAsync();
                            MessageBox.Show("Xác nhận thanh toán thành công.");
                            ChiTietVe.TrangThai = "Đã thanh toán";
                            OnPropertyChanged(nameof(ChiTietVe));
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy vé để xác nhận thanh toán.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xác nhận thanh toán: {ex.Message}");
                }
            }
        }
    }
}
