using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
                    var khachHang = context.Khachhangs.FirstOrDefault(kh => kh.MaKh == UserSession.Current.CustomerId);
                    if (khachHang != null)
                    {
                        HoTen = khachHang.HoTenKh;
                        SoDienThoai = khachHang.SoDt;
                        Email = khachHang.Email;
                        MaKhachHang = khachHang.MaKh;
                        CanCuoc = khachHang.GiayToTt;
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
        private void SaveProfile()
        {
            try
            {
                using (var context = new AirTicketDbContext())
                {
                    var khachHang = context.Khachhangs.FirstOrDefault(kh => kh.MaKh == UserSession.Current.CustomerId);
                    if (khachHang != null)
                    {
                        // Họ tên: bắt buộc phải nhập
                        if (string.IsNullOrWhiteSpace(EditHoTen))
                        {
                            MessageBox.Show("Họ tên không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                            EditHoTen = HoTen;
                            return;
                        }
                        khachHang.HoTenKh = EditHoTen;

                        // Email: nếu có nhập thì phải đúng định dạng
                        if (string.IsNullOrWhiteSpace(EditEmail))
                        {
                            MessageBox.Show("Email không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                            EditEmail = Email;

                            return;
                        }
                        else
                        {
                            if (!IsValidEmail(EditEmail))
                            {
                                MessageBox.Show("Email không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                                EditEmail = Email;
                                return;
                            }
                            khachHang.Email = EditEmail;
                        }

                        // Số điện thoại: nếu có nhập thì kiểm tra định dạng
                        if (!string.IsNullOrWhiteSpace(EditSoDienThoai))
                        {
                            if (!IsValidPhone(EditSoDienThoai))
                            {
                                MessageBox.Show("Số điện thoại không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                        }
                        khachHang.SoDt = EditSoDienThoai;


                        // Căn cước: nếu có nhập thì kiểm tra độ dài hợp lệ (ví dụ 9 hoặc 12 số)
                        if (!string.IsNullOrWhiteSpace(EditCanCuoc))
                        {
                            if (EditCanCuoc.Length != 12)
                            {
                                MessageBox.Show("Số căn cước phải có 12 chữ số!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                        }
                        khachHang.GiayToTt = EditCanCuoc;


                        // Giới tính: nếu có nhập thì lưu
                        if (!string.IsNullOrWhiteSpace(EditGioiTinh))
                        {
                            khachHang.GioiTinh = EditGioiTinh;
                        }

                        // Ngày sinh: nếu có nhập thì phải nhỏ hơn ngày hiện tại
                        if (EditNgaySinh.HasValue)
                        {
                            if (EditNgaySinh.Value.Date >= DateTime.Today)
                            {
                                MessageBox.Show("Ngày sinh không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                            khachHang.NgaySinh = DateOnly.FromDateTime(EditNgaySinh.Value);
                        }
                        else
                        {
                            khachHang.NgaySinh = null;
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
    }
}
