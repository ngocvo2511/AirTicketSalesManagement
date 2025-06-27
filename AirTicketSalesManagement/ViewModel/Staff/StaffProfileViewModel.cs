using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
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

namespace AirTicketSalesManagement.ViewModel.Staff
{
    public partial class StaffProfileViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string hoTen;
        [ObservableProperty]
        private string soDienThoai;
        [ObservableProperty]
        private string email;
        [ObservableProperty]
        private string maNhanVien;
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



        public StaffProfileViewModel()
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (var context = new AirTicketDbContext())
                {
                    var nhanVien = context.Nhanviens
                        .Include(nv => nv.Taikhoans)
                        .FirstOrDefault(nv => nv.MaNv == UserSession.Current.StaffId);
                    if (nhanVien != null)
                    {
                        HoTen = nhanVien.HoTenNv;
                        SoDienThoai = nhanVien.SoDt;
                        Email = nhanVien.Taikhoans.FirstOrDefault().Email;
                        MaNhanVien = nhanVien.MaNv.ToString();
                        CanCuoc = nhanVien.Cccd;
                        GioiTinh = nhanVien.GioiTinh;
                        if (nhanVien.NgaySinh.HasValue)
                        {
                            NgaySinh = nhanVien.NgaySinh.Value.ToDateTime(TimeOnly.MinValue);
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
        private void SaveProfile()
        {
            try
            {
                using (var context = new AirTicketDbContext())
                {
                    var nhanVien = context.Nhanviens
                        .Include(nv => nv.Taikhoans)
                        .FirstOrDefault(kh => kh.MaNv == UserSession.Current.StaffId);
                    if (nhanVien != null)
                    {
                        // Họ tên: bắt buộc phải nhập
                        if (string.IsNullOrWhiteSpace(EditHoTen))
                        {
                            MessageBox.Show("Họ tên không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                            EditHoTen = HoTen;
                            return;
                        }
                        nhanVien.HoTenNv = EditHoTen;

                        // Email: nếu có nhập thì phải đúng định dạng
                        if (string.IsNullOrWhiteSpace(EditEmail))
                        {
                            MessageBox.Show("Email không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                            EditEmail = Email;
                            return;
                        }

                        if (!IsValidEmail(EditEmail))
                        {
                            MessageBox.Show("Email không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                            EditEmail = Email;
                            return;
                        }

                        bool emailExists = context.Taikhoans
                            .Any(tk => tk.Email == EditEmail && tk.MaNv != nhanVien.MaNv);

                        if (emailExists)
                        {
                            MessageBox.Show("Email đã được sử dụng bởi tài khoản khác!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                            EditEmail = Email;
                            return;
                        }

                        nhanVien.Taikhoans.FirstOrDefault().Email = EditEmail;


                        // Số điện thoại: nếu có nhập thì kiểm tra định dạng
                        if (!string.IsNullOrWhiteSpace(EditSoDienThoai))
                        {
                            if (!IsValidPhone(EditSoDienThoai))
                            {
                                MessageBox.Show("Số điện thoại không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                                EditSoDienThoai = SoDienThoai;
                                return;
                            }
                        }
                        nhanVien.SoDt = EditSoDienThoai;


                        // Căn cước: nếu có nhập thì kiểm tra độ dài hợp lệ (ví dụ 9 hoặc 12 số)
                        if (!string.IsNullOrWhiteSpace(EditCanCuoc))
                        {
                            if (EditCanCuoc.Length != 12 || !EditCanCuoc.All(char.IsDigit))
                            {
                                MessageBox.Show("Số căn cước không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                                EditCanCuoc = CanCuoc;
                                return;
                            }
                        }
                        nhanVien.Cccd = EditCanCuoc;


                        // Giới tính: nếu có nhập thì lưu
                        if (!string.IsNullOrWhiteSpace(EditGioiTinh))
                        {
                            nhanVien.GioiTinh = EditGioiTinh;
                        }

                        // Ngày sinh: nếu có nhập thì phải nhỏ hơn ngày hiện tại
                        if (EditNgaySinh.HasValue)
                        {
                            if (EditNgaySinh.Value.Date >= DateTime.Today)
                            {
                                MessageBox.Show("Ngày sinh không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                                EditNgaySinh = NgaySinh;
                                return;
                            }
                            nhanVien.NgaySinh = DateOnly.FromDateTime(EditNgaySinh.Value);
                        }
                        else
                        {
                            nhanVien.NgaySinh = null;
                        }


                        context.SaveChanges();
                        MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                        IsEditPopupOpen = false; // Đóng popup sau khi lưu thành công
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private void ChangePassword()
        {
            HideError();
            try
            {
                using (var context = new AirTicketDbContext())
                {

                    var account = context.Taikhoans.FirstOrDefault(tk => tk.MaNv == UserSession.Current.StaffId);
                    if (account == null)
                    {
                        MessageBox.Show("Không tìm thấy tài khoản.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        MessageBox.Show("Mật khẩu mới không khớp hoặc trống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Mã hóa mật khẩu mới
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    account.MatKhau = hashedPassword;

                    context.SaveChanges();

                    MessageBox.Show("Đổi mật khẩu thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Xóa các trường để tránh lộ mật khẩu
                    CurrentPassword = string.Empty;
                    NewPassword = string.Empty;
                    ConfirmPassword = string.Empty;
                    IsChangePasswordPopupOpen = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi đổi mật khẩu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
