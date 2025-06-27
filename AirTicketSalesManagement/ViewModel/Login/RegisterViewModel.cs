using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using System.Windows.Media;
using System.Diagnostics;

namespace AirTicketSalesManagement.ViewModel.Login
{
    public partial class RegisterViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        private readonly AuthViewModel _auth;
        private readonly Dictionary<string, List<string>> _errors = new();
        public ToastViewModel Toast { get; } = new ToastViewModel();
        private bool isFailed;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;
        [ObservableProperty]
        private string confirmPassword;
        [ObservableProperty]
        private string name;

        #region Error
        public async Task Validate()
        {
            ClearErrors(nameof(Email));
            ClearErrors(nameof(Password));
            ClearErrors(nameof(ConfirmPassword));
            ClearErrors(nameof(Name));
            if (string.IsNullOrWhiteSpace(Name))
                AddError(nameof(Name), "Tên không được để trống.");
            else if (Name.Length > 30)
                AddError(nameof(Name), "Tên vượt quá giới hạn cho phép");
            if (string.IsNullOrWhiteSpace(Email))
                AddError(nameof(Email), "Email không được để trống.");
            else if (Email.Length > 254)
                AddError(nameof(Name), "Email vượt quá giới hạn cho phép");
            else if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                AddError(nameof(Email), "Email không hợp lệ.");
            
            if (string.IsNullOrWhiteSpace(Password))
                AddError(nameof(Password), "Mật khẩu không được để trống.");
            else if (Password.Length > 100)
                AddError(nameof(Password), "Mật khẩu vượt quá giới hạn cho phép");
            if (ConfirmPassword != Password)
            {
                AddError(nameof(ConfirmPassword), "Xác nhận mật khẩu không khớp với mật khẩu.");
            }
            if (HasErrors) return;
            try
            {
                using (var context = new AirTicketDbContext())
                {
                    var user = context.Taikhoans.FirstOrDefault(x => x.Email == Email);
                    if (user != null)
                    {
                        AddError(nameof(Email), "Email đã được đăng kí");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                await Toast.ShowToastAsync("Không thể kết nối đến cơ sở dữ liệu", Brushes.OrangeRed);
            }
            
        }
        public bool HasErrors => _errors.Any();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public IEnumerable? GetErrors(string propertyName)
        {
            if (!string.IsNullOrWhiteSpace(propertyName) && _errors.ContainsKey(propertyName))
                return _errors[propertyName];
            return null;
        }

        private void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (!_errors[propertyName].Contains(error))
            {
                _errors[propertyName].Add(error);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        private void ClearErrors(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region add account
       
        public async Task AddCustomer()
        {
            if (isFailed)
            {
                await Toast.ShowToastAsync("Không thể kết nối đến cơ sở dữ liệu", Brushes.OrangeRed);
                return;
            }
            try
            {
                using (var context = new AirTicketDbContext())
                {
                    string hashPass = BCrypt.Net.BCrypt.HashPassword(Password);
                    var customer = new Khachhang
                    {
                        HoTenKh = Name
                    };
                    var customerAccount = new Taikhoan
                    {
                        Email = Email,
                        VaiTro = "Khách hàng",
                        MatKhau = hashPass,
                        MaKhNavigation = customer
                    };
                    context.Khachhangs.Add(customer);
                    context.Taikhoans.Add(customerAccount);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                isFailed = true;
                await Toast.ShowToastAsync("Không thể kết nối đến cơ sở dữ liệu", Brushes.OrangeRed);
                return;
            }
           
        }
        #endregion
        public RegisterViewModel()
        {
            // Default constructor
        }

        public RegisterViewModel(AuthViewModel auth)
        {
            _auth = auth;
        }

        [RelayCommand]
        private async Task Register()
        {
            await Validate();
            if (HasErrors) return;
            await AddCustomer();
            if (!isFailed) 
            {
                await Toast.ShowToastAsync("Đăng kí tài khoản thành công");
            }
        }

        [RelayCommand]
        private void ShowLogin() => _auth.NavigateToLogin();
    }
}
