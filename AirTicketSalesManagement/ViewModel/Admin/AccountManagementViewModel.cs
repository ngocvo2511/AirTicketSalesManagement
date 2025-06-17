using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.Models.UIModels;
using AirTicketSalesManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace AirTicketSalesManagement.ViewModel.Admin
{
    public partial class AccountManagementViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string searchEmail;

        [ObservableProperty]
        private string searchRole;

        [ObservableProperty]
        private ObservableCollection<AccountModel> accounts = new();

        [ObservableProperty]
        private AccountModel selectedAccount;

        // Properties for Add Account
        [ObservableProperty]
        private string addEmail;

        [ObservableProperty]
        private string addRole;

        [ObservableProperty]
        private string addPassword;

        [ObservableProperty]
        private string addFullName;


        [ObservableProperty]
        private UserSelectionModel selectedUser;

        [ObservableProperty]
        private bool isAddPopupOpen = false;


        // Properties for Edit Account
        [ObservableProperty]
        private string editEmail;

        [ObservableProperty]
        private string editRole;

        [ObservableProperty]
        private string editPassword;

        [ObservableProperty]
        private string editFullName;

        [ObservableProperty]
        private UserSelectionModel editSelectedUser;

        [ObservableProperty]
        private bool isEditPopupOpen = false;

        // Collections for ComboBoxes
        [ObservableProperty]
        private ObservableCollection<UserSelectionModel> userList = new();

        public AccountManagementViewModel()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                LoadAccounts();
                LoadUserList();
            }
        }

        public void LoadAccounts()
        {
            using var context = new AirTicketDbContext();
            var danhSach = context.Taikhoans
                .Include(tk => tk.MaNvNavigation)
                .Include(tk => tk.MaKhNavigation)
                .AsEnumerable()
                .Select(tk => AccountModel.FromTaiKhoan(tk))
                .ToList();

            Accounts = new ObservableCollection<AccountModel>(danhSach);
        }

        private void LoadUserList()
        {
            using var context = new AirTicketDbContext();
            var nhanVienList = context.Nhanviens
                .Select(nv => UserSelectionModel.FromNhanVien(nv))
                .ToList();

            var khachHangList = context.Khachhangs
                .Select(kh => UserSelectionModel.FromKhachHang(kh))
                .ToList();

            UserList = new ObservableCollection<UserSelectionModel>(
                nhanVienList.Concat(khachHangList).OrderBy(u => u.Name));
        }

        [RelayCommand]
        public void Refresh()
        {
            LoadAccounts();
        }

        [RelayCommand]
        public void ClearSearch()
        {
            SearchEmail = string.Empty;
            SearchRole = string.Empty;
            LoadAccounts();
        }

        [RelayCommand]
        public void Search()
        {
            Accounts.Clear();

            using (var context = new AirTicketDbContext())
            {
                var query = context.Taikhoans
                    .Include(tk => tk.MaNvNavigation)
                    .Include(tk => tk.MaKhNavigation)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(SearchEmail))
                {
                    query = query.Where(tk => tk.Email.Contains(SearchEmail));
                }

                if (!string.IsNullOrWhiteSpace(SearchRole) && SearchRole != "Tất cả")
                {
                    query = query.Where(tk => tk.VaiTro == SearchRole);
                }

                var results = query.ToList();
                foreach (var tk in results)
                {
                    Accounts.Add(AccountModel.FromTaiKhoan(tk));
                }
            }
        }

        [RelayCommand]
        public void AddAccount()
        {
            ResetAddFields();
            IsAddPopupOpen = true;
        }

        private void ResetAddFields()
        {
            AddEmail = string.Empty;
            AddRole = string.Empty;
            AddFullName = string.Empty;
            AddPassword = string.Empty;
        }

        [RelayCommand]
        public void CancelAdd()
        {
            IsAddPopupOpen = false;
        }

        [RelayCommand]
        public void CloseAdd()
        {
            IsAddPopupOpen = false;
        }

        [RelayCommand]
        public void SaveAddAccount()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(AddEmail) || string.IsNullOrWhiteSpace(AddRole) || string.IsNullOrWhiteSpace(AddPassword) || string.IsNullOrWhiteSpace(AddFullName))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin tài khoản.",
                                  "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                using (var context = new AirTicketDbContext())
                {
                    // Check if email already exists
                    if (context.Taikhoans.Any(tk => tk.Email == AddEmail))
                    {
                        MessageBox.Show("Email này đã được sử dụng.",
                                      "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Create new user if needed
                    int? maNv = null;
                    int? maKh = null;

                    if (AddRole == "Nhân viên" || AddRole == "Admin")
                    {
                        var newNhanVien = new Nhanvien
                        {
                            HoTenNv = AddFullName,
                        };

                        context.Nhanviens.Add(newNhanVien);
                        context.SaveChanges();
                        maNv = newNhanVien.MaNv;
                    }
                    else if (AddRole == "Khách hàng")
                    {
                        var newKhachHang = new Khachhang
                        {
                            HoTenKh = AddFullName,
                        };

                        context.Khachhangs.Add(newKhachHang);
                        context.SaveChanges();
                        maKh = newKhachHang.MaKh;
                    }

                    var newAccount = new Taikhoan
                    {
                        Email = AddEmail,
                        MatKhau = BCrypt.Net.BCrypt.HashPassword(AddPassword),
                        VaiTro = AddRole,
                        MaNv = maNv,
                        MaKh = maKh
                    };

                    context.Taikhoans.Add(newAccount);
                    context.SaveChanges();

                    MessageBox.Show("Tài khoản đã được thêm thành công!",
                                  "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    IsAddPopupOpen = false;
                    LoadAccounts();
                    LoadUserList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi thêm tài khoản: " + ex.Message,
                              "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        [RelayCommand]
        public void EditAccount()
        {
            if (SelectedAccount == null) return;

            ResetEditFields();
            IsEditPopupOpen = true;
        }

        private void ResetEditFields()
        {
            EditEmail = SelectedAccount.Email;
            EditRole = SelectedAccount.VaiTro;
            EditFullName = SelectedAccount.HoTen;

            // Find and set the selected user
            if (SelectedAccount.MaNv.HasValue)
            {
                EditSelectedUser = UserList.FirstOrDefault(u => 
                    u.Id == SelectedAccount.MaNv.Value && u.Type == "Nhân viên");
            }
            else if (SelectedAccount.MaKh.HasValue)
            {
                EditSelectedUser = UserList.FirstOrDefault(u => 
                    u.Id == SelectedAccount.MaKh.Value && u.Type == "Khách hàng");
            }
        }

        [RelayCommand]
        public void CancelEdit()
        {
            IsEditPopupOpen = false;
        }

        [RelayCommand]
        public void CloseEdit()
        {
            IsEditPopupOpen = false;
        }

        [RelayCommand]
        public void SaveEditAccount()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EditEmail) || string.IsNullOrWhiteSpace(EditRole) || string.IsNullOrWhiteSpace(EditFullName))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin tài khoản.",
                                  "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                using (var context = new AirTicketDbContext())
                {
                    var existingAccount = context.Taikhoans
                        .FirstOrDefault(tk => tk.MaTk == SelectedAccount.Id);

                    if (existingAccount == null)
                    {
                        MessageBox.Show("Không tìm thấy tài khoản để chỉnh sửa.",
                                      "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!IsValidEmail(EditEmail))
                    {
                        MessageBox.Show("Email không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    if (existingAccount.Email != EditEmail)
                    {
                        if (context.Taikhoans.Any(tk => tk.Email == EditEmail))
                        {
                            MessageBox.Show("Email này đã được sử dụng.",
                                          "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        existingAccount.Email = EditEmail;
                    }

                    if (!string.IsNullOrEmpty(EditPassword))
                    {
                        existingAccount.MatKhau = BCrypt.Net.BCrypt.HashPassword(EditPassword);
                    }

                    if (existingAccount.MaTk == UserSession.Current.AccountId && existingAccount.VaiTro != EditRole)
                    {
                        MessageBox.Show("Không thể chỉnh sửa vai trò tài khoản của bạn.",
                                          "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    existingAccount.VaiTro = EditRole;

                    if (EditRole == "Nhân viên" || EditRole == "Admin" && EditSelectedUser != null)
                    {
                        if (existingAccount.MaKh != null) //neu la khach hang -> tao nv moi
                        {
                            var oldCustomer = context.Khachhangs.Find(existingAccount.MaKh);
                            if (oldCustomer != null)
                            {
                                var newNhanVien = new Nhanvien
                                {
                                    HoTenNv = EditFullName,
                                    GioiTinh = oldCustomer.GioiTinh,
                                    NgaySinh = oldCustomer.NgaySinh,
                                    SoDt = oldCustomer.SoDt,
                                    Cccd = oldCustomer.Cccd
                                };
                                context.Nhanviens.Add(newNhanVien);
                                context.SaveChanges(); // Để lấy được MaNV mới

                                existingAccount.MaNv = newNhanVien.MaNv;
                                existingAccount.MaKh = null;
                            }
                        }
                        else
                        {
                            var staff = context.Nhanviens.Find(existingAccount.MaNv);
                            staff.HoTenNv = EditFullName;
                        }
                    }
                    else if (EditRole == "Khách hàng" && EditSelectedUser != null)
                    {
                        if (existingAccount.MaNv != null)
                        {
                            var oldNhanVien = context.Nhanviens.Find(existingAccount.MaNv);
                            if (oldNhanVien != null)
                            {
                                var newKhachHang = new Khachhang
                                {
                                    HoTenKh = oldNhanVien.HoTenNv,
                                    GioiTinh = oldNhanVien.GioiTinh,
                                    NgaySinh = oldNhanVien.NgaySinh,
                                    SoDt = oldNhanVien.SoDt,
                                    Cccd = oldNhanVien.Cccd
                                };
                                context.Khachhangs.Add(newKhachHang);
                                context.SaveChanges(); // Để lấy MaKH mới

                                existingAccount.MaKh = newKhachHang.MaKh;
                                existingAccount.MaNv = null;
                            }
                        }
                        else
                        {
                            var customer = context.Khachhangs.Find(existingAccount.MaKh);
                            customer.HoTenKh = EditFullName;
                        }
                    }

                    context.SaveChanges();

                    MessageBox.Show("Tài khoản đã được cập nhật thành công!",
                                  "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    IsEditPopupOpen = false;
                    LoadAccounts();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi cập nhật tài khoản: " + ex.Message,
                              "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        public void DeleteAccount()
        {
            if (SelectedAccount == null)
            {
                MessageBox.Show("Vui lòng chọn một tài khoản để xóa.",
                              "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa tài khoản {SelectedAccount.Email}?",
                                       "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                using (var context = new AirTicketDbContext())
                {
                    var account = context.Taikhoans
                        .FirstOrDefault(tk => tk.Email == SelectedAccount.Email);

                    if (account == null)
                    {
                        MessageBox.Show("Không tìm thấy tài khoản trong cơ sở dữ liệu.",
                                      "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    context.Taikhoans.Remove(account);
                    context.SaveChanges();

                    MessageBox.Show("Đã xóa tài khoản thành công!",
                                  "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadAccounts();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi xóa tài khoản: " + ex.Message,
                              "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
