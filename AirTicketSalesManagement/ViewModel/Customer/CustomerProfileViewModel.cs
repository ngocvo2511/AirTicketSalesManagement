﻿using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AirTicketSalesManagement.ViewModel.Customer
{
    public partial class CustomerProfileViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string hoTen;
        [ObservableProperty]
        private string soDienThoai;
        [ObservableProperty]
        private string email;
        [ObservableProperty]
        private string maKhachHang;
        [ObservableProperty]
        private string canCuoc;
        [ObservableProperty]
        private string gioiTinh;
        [ObservableProperty]
        private DateTime? ngaySinh;
        [ObservableProperty]
        private bool isEditPopupOpen;
        [ObservableProperty]
        private bool isChangePasswordPopupOpen;
        [ObservableProperty]
        private bool isPasswordVisible;

        [ObservableProperty]
        private string editHoTen;
        [ObservableProperty]
        private string editSoDienThoai;
        [ObservableProperty]
        private string editEmail;
        [ObservableProperty]
        private string editCanCuoc;
        [ObservableProperty]
        private string editGioiTinh;
        [ObservableProperty]
        private DateTime? editNgaySinh;

        [ObservableProperty]
        private string currentPassword;
        [ObservableProperty]
        private string newPassword;
        [ObservableProperty]
        private string confirmPassword;
        [ObservableProperty]
        private bool hasPasswordError = false;
        [ObservableProperty]
        private string passwordErrorMessage;

        // Notification
        public NotificationViewModel Notification { get; } = new NotificationViewModel();

        public CustomerProfileViewModel()
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (var context = new AirTicketDbContext())
                {
                    var khachHang = context.Khachhangs
                        .Include(kh => kh.Taikhoans)
                        .FirstOrDefault(kh => kh.MaKh == UserSession.Current.CustomerId);
                    if (khachHang != null)
                    {
                        HoTen = khachHang.HoTenKh;
                        SoDienThoai = khachHang.SoDt;
                        Email = khachHang.Taikhoans.FirstOrDefault().Email;
                        MaKhachHang = khachHang.MaKh.ToString();
                        CanCuoc = khachHang.Cccd;
                        GioiTinh = khachHang.GioiTinh;
                        if (khachHang.NgaySinh.HasValue)
                        {
                            NgaySinh = khachHang.NgaySinh.Value.ToDateTime(TimeOnly.MinValue);
                        }
                        else
                        {
                            NgaySinh = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần
                Console.WriteLine($"Lỗi khi tải dữ liệu: {ex.Message}");
            }
        }

        [RelayCommand]
        private void OpenEditProfile()
        {
            ResetField();
            IsEditPopupOpen = true;
        }

        private void ResetField()
        {
            EditHoTen = HoTen;
            EditSoDienThoai = SoDienThoai;
            EditCanCuoc = CanCuoc;
            EditGioiTinh = GioiTinh;
            EditNgaySinh = NgaySinh;
            EditEmail = Email;
        }

        [RelayCommand]
        private void CloseEditPopup()
        {
            IsEditPopupOpen = false;
        }

        [RelayCommand]
        private async void SaveProfile()
        {
            try
            {
                using (var context = new AirTicketDbContext())
                {
                    var khachhang = context.Khachhangs
                        .Include(nv => nv.Taikhoans)
                        .FirstOrDefault(kh => kh.MaKh == UserSession.Current.CustomerId);
                    if (khachhang != null)
                    {
                        // Họ tên: bắt buộc phải nhập
                        if (string.IsNullOrWhiteSpace(EditHoTen))
                        {
                            await Notification.ShowNotificationAsync("Họ tên không được để trống!", NotificationType.Warning);
                            EditHoTen = HoTen;
                            return;
                        }
                        khachhang.HoTenKh = EditHoTen;

                        // Email: nếu có nhập thì phải đúng định dạng
                        if (string.IsNullOrWhiteSpace(EditEmail))
                        {
                            await Notification.ShowNotificationAsync("Email không được để trống!", NotificationType.Warning);
                            EditEmail = Email;
                            return;
                        }

                        if (!IsValidEmail(EditEmail))
                        {
                            await Notification.ShowNotificationAsync("Email không hợp lệ!", NotificationType.Warning);
                            EditEmail = Email;
                            return;
                        }

                        bool emailExists = context.Taikhoans
                            .Any(tk => tk.Email == EditEmail && tk.MaNv != khachhang.MaKh);

                        if (emailExists)
                        {
                            await Notification.ShowNotificationAsync("Email đã được sử dụng bởi tài khoản khác!", NotificationType.Warning);
                            EditEmail = Email;
                            return;
                        }

                        khachhang.Taikhoans.FirstOrDefault().Email = EditEmail;

                        // Số điện thoại: nếu có nhập thì kiểm tra định dạng
                        if (!string.IsNullOrWhiteSpace(EditSoDienThoai))
                        {
                            if (!IsValidPhone(EditSoDienThoai))
                            {
                                await Notification.ShowNotificationAsync("Số điện thoại không hợp lệ!", NotificationType.Warning);
                                EditSoDienThoai = SoDienThoai;
                                return;
                            }
                        }
                        khachhang.SoDt = EditSoDienThoai;

                        // Căn cước: nếu có nhập thì kiểm tra độ dài hợp lệ (ví dụ 9 hoặc 12 số)
                        if (!string.IsNullOrWhiteSpace(EditCanCuoc))
                        {
                            if (EditCanCuoc.Length != 12 || !EditCanCuoc.All(char.IsDigit))
                            {
                                await Notification.ShowNotificationAsync("Số căn cước không hợp lệ!", NotificationType.Warning);
                                EditCanCuoc = CanCuoc;
                                return;
                            }
                        }
                        khachhang.Cccd = EditCanCuoc;

                        // Giới tính: nếu có nhập thì lưu
                        if (!string.IsNullOrWhiteSpace(EditGioiTinh))
                        {
                            khachhang.GioiTinh = EditGioiTinh;
                        }

                        // Ngày sinh: nếu có nhập thì phải nhỏ hơn ngày hiện tại
                        if (EditNgaySinh.HasValue)
                        {
                            if (EditNgaySinh.Value.Date >= DateTime.Today)
                            {
                                await Notification.ShowNotificationAsync("Ngày sinh không hợp lệ!", NotificationType.Warning);
                                EditNgaySinh = NgaySinh;
                                return;
                            }
                            khachhang.NgaySinh = DateOnly.FromDateTime(EditNgaySinh.Value);
                        }
                        else
                        {
                            khachhang.NgaySinh = null;
                        }

                        context.SaveChanges();
                        await Notification.ShowNotificationAsync("Cập nhật thông tin thành công!", NotificationType.Information);
                        LoadData();
                        IsEditPopupOpen = false; // Đóng popup sau khi lưu thành công
                    }
                }
            }
            catch (Exception ex)
            {
                await Notification.ShowNotificationAsync($"Đã xảy ra lỗi: {ex.Message}", NotificationType.Error);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            // Ví dụ: bắt đầu bằng 0, từ 9–11 chữ số
            return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^0\d{8,10}$");
        }

        [RelayCommand]
        private void OpenChangePassword()
        {
            ResetPasswordField();
            IsChangePasswordPopupOpen = true;
        }

        [RelayCommand]
        private void CloseChangePasswordPopup()
        {
            IsChangePasswordPopupOpen = false;
        }

        [RelayCommand]
        private async void ChangePassword()
        {
            HideError();
            try
            {
                using (var context = new AirTicketDbContext())
                {
                    var account = context.Taikhoans.FirstOrDefault(tk => tk.MaKh == UserSession.Current.CustomerId);
                    if (account == null)
                    {
                        await Notification.ShowNotificationAsync("Không tìm thấy tài khoản.", NotificationType.Error);
                        return;
                    }

                    // Kiểm tra mật khẩu hiện tại
                    if (!BCrypt.Net.BCrypt.Verify(currentPassword, account.MatKhau))
                    {
                        ShowError("Mật khẩu hiện tại không đúng.");
                        return;
                    }

                    // Kiểm tra xác nhận mật khẩu mới
                    if (string.IsNullOrWhiteSpace(newPassword) || newPassword != confirmPassword)
                    {
                        await Notification.ShowNotificationAsync("Mật khẩu mới không khớp hoặc trống.", NotificationType.Warning);
                        return;
                    }

                    // Mã hóa mật khẩu mới
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    account.MatKhau = hashedPassword;

                    context.SaveChanges();

                    await Notification.ShowNotificationAsync("Đổi mật khẩu thành công.", NotificationType.Information);

                    // Xóa các trường để tránh lộ mật khẩu
                    CurrentPassword = string.Empty;
                    NewPassword = string.Empty;
                    ConfirmPassword = string.Empty;
                    IsChangePasswordPopupOpen = false;
                }
            }
            catch (Exception ex)
            {
                await Notification.ShowNotificationAsync($"Có lỗi xảy ra khi đổi mật khẩu: {ex.Message}", NotificationType.Error);
            }
        }

        private void ResetPasswordField()
        {
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
            HasPasswordError = false;
            PasswordErrorMessage = string.Empty;
        }

        private void ShowError(string Error)
        {
            PasswordErrorMessage = Error;
            HasPasswordError = true;
        }

        private void HideError()
        {
            HasPasswordError = false;
            PasswordErrorMessage = string.Empty;
        }
    }
}