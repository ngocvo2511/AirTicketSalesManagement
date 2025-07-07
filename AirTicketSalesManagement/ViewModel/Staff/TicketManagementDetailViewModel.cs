﻿using AirTicketSalesManagement.Data;
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
using System.Windows.Media;

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
        [ObservableProperty]
        private NotificationViewModel notification = new();

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
            if (parent is StaffViewModel staffViewModel)
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
                await notification.ShowNotificationAsync(
                    "Vé đã được hủy trước đó.",
                    NotificationType.Warning);
                return;
            }
            if (ChiTietVe.CanCancel == false)
            {
                await notification.ShowNotificationAsync(
                    "Vé không thể hủy do đã quá thời gian hủy.",
                    NotificationType.Warning);
                return;
            }
            bool confirm = await notification.ShowNotificationAsync(
                "Bạn có chắc chắn muốn hủy vé này không?",
                NotificationType.Information,
                true);
            if (confirm)
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
                            await notification.ShowNotificationAsync(
                                "Hủy vé thành công.",
                                NotificationType.Information);
                            ChiTietVe.TrangThai = "Đã hủy";
                            OnPropertyChanged(nameof(ChiTietVe));
                        }
                        else
                        {
                            await notification.ShowNotificationAsync(
                                "Không tìm thấy vé để hủy.",
                                NotificationType.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    await notification.ShowNotificationAsync(
                        $"Lỗi khi hủy vé: {ex.Message}",
                        NotificationType.Error);
                }
            }
        }
        [RelayCommand]
        private async Task ConfirmPayment()
        {
            if (ChiTietVe.TrangThai != "Chờ thanh toán")
            {
                await notification.ShowNotificationAsync(
                    "Không thể xác nhận thanh toán.",
                    NotificationType.Warning);
                return;
            }
            if (ChiTietVe.CanConfirm == false)
            {
                await notification.ShowNotificationAsync(
                    "Vé không thể thanh toán do đã quá thời gian đặt vé.",
                    NotificationType.Warning);
                return;
            }
            bool confirm = await notification.ShowNotificationAsync(
                "Bạn có chắc chắn muốn xác nhận thanh toán vé này không?",
                NotificationType.Information,
                true);
            if (confirm)
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
                            await notification.ShowNotificationAsync(
                                "Xác nhận thanh toán thành công.",
                                NotificationType.Information);
                            ChiTietVe.TrangThai = "Đã thanh toán";
                            OnPropertyChanged(nameof(ChiTietVe));
                        }
                        else
                        {
                            await notification.ShowNotificationAsync(
                                "Không tìm thấy vé để xác nhận thanh toán.",
                                NotificationType.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    await notification.ShowNotificationAsync(
                        $"Lỗi khi xác nhận thanh toán: {ex.Message}",
                        NotificationType.Error);
                }
            }
        }
    }
}