using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.Services;
using AirTicketSalesManagement.View.Admin;
using AirTicketSalesManagement.View.Customer;
using AirTicketSalesManagement.View.Login;
using AirTicketSalesManagement.View.Staff;
using AirTicketSalesManagement.ViewModel.Admin;
using AirTicketSalesManagement.ViewModel.Customer;
using AirTicketSalesManagement.ViewModel.Staff;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace AirTicketSalesManagement.ViewModel.Login
{
    public partial class LoginViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        private readonly AuthViewModel _auth;
        private readonly Dictionary<string, List<string>> _errors = new();
        public ToastViewModel Toast { get; } = new ToastViewModel();

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isPasswordVisible;


        #region Error
        public void Validate()
        {
            ClearErrors(nameof(Email));

            if (string.IsNullOrWhiteSpace(Email) || !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$") || string.IsNullOrWhiteSpace(Password))
            {
                AddError(nameof(Email), "Tài khoản hoặc mật khẩu không hợp lệ");
                return;
            }            
        }

        public bool HasErrors => _errors.Any();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public IEnumerable GetErrors(string propertyName)
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

        public LoginViewModel()
        {
            // Default constructor
        }
        public LoginViewModel(AuthViewModel auth)
        {
            _auth = auth;
        }

        [RelayCommand]
        private async Task Login()
        {
            Validate();
            if (HasErrors) return;
            try
            {
                using (var context = new AirTicketDbContext())
                {
                    var user = context.Taikhoans.FirstOrDefault(x => x.Email == Email);
                    if (user == null || !BCrypt.Net.BCrypt.Verify(Password, user.MatKhau))
                    {
                        AddError(nameof(Email), "Tài khoản hoặc mật khẩu không hợp lệ");
                        return;
                    }
                    else
                    {
                        if (user.VaiTro == "Khách hàng")
                        {
                            var khachHang = context.Khachhangs.FirstOrDefault(kh => kh.MaKh == user.MaKh);
                            if (khachHang == null)
                            {
                                AddError(nameof(Email), "Không tìm thấy thông tin khách hàng.");
                                return;
                            }
                            UserSession.Current.AccountId = user.MaTk;
                            UserSession.Current.CustomerId = user.MaKh;
                            UserSession.Current.StaffId = null; 
                            UserSession.Current.CustomerName = khachHang.HoTenKh;
                            UserSession.Current.isStaff = false;
                            UserSession.Current.Email = user.Email;

                            var currentWindow = Application.Current.MainWindow;
                            var vm = new Customer.CustomerViewModel();
                            vm.IdCustomer = user.MaKh;


                            //MessageBox.Show(UserSession.Current.CustomerId + " " + UserSession.Current.CustomerName);

                            var customerWindow = new CustomerWindow
                            {
                                DataContext = vm
                            };
                            Application.Current.MainWindow = customerWindow;
                            currentWindow?.Close();
                            customerWindow.Opacity = 0;
                            customerWindow.Show();
                            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(270));
                            customerWindow.BeginAnimation(Window.OpacityProperty, fadeIn);
                        }
                        else if (user.VaiTro == "Nhân viên")
                        {
                            var nhanvien = context.Nhanviens.FirstOrDefault(nv => nv.MaNv == user.MaNv);
                            if (nhanvien == null)
                            {
                                AddError(nameof(Email), "Không tìm thấy thông tin nhân viên.");
                                return;
                            }
                            UserSession.Current.AccountId = user.MaTk;
                            UserSession.Current.CustomerId = null; 
                            UserSession.Current.StaffId = user.MaNv;
                            UserSession.Current.CustomerName = nhanvien.HoTenNv;
                            UserSession.Current.isStaff = true;
                            UserSession.Current.Email = user.Email;


                            var currentWindow = Application.Current.MainWindow;
                            var vm = new StaffViewModel();


                            //MessageBox.Show(UserSession.Current.CustomerId + " " + UserSession.Current.CustomerName);

                            var staffWindow = new StaffWindow
                            {
                                DataContext = vm
                            };
                            Application.Current.MainWindow = staffWindow;
                            currentWindow?.Close();
                            staffWindow.Opacity = 0;
                            staffWindow.Show();
                            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(270));
                            staffWindow.BeginAnimation(Window.OpacityProperty, fadeIn);
                        }
                        else if (user.VaiTro == "Admin")
                        {
                            var nhanvien = context.Nhanviens.FirstOrDefault(nv => nv.MaNv == user.MaNv);
                            if (nhanvien == null)
                            {
                                AddError(nameof(Email), "Không tìm thấy thông tin nhân viên.");
                                return;
                            }
                            UserSession.Current.AccountId = user.MaTk;
                            UserSession.Current.CustomerId = null;
                            UserSession.Current.StaffId = user.MaNv;
                            UserSession.Current.CustomerName = nhanvien.HoTenNv;
                            UserSession.Current.isStaff = true;
                            UserSession.Current.Email = user.Email;


                            var currentWindow = Application.Current.MainWindow;
                            var vm = new AdminViewModel();


                            //MessageBox.Show(UserSession.Current.CustomerId + " " + UserSession.Current.CustomerName);

                            var adminWindow = new AdminWindow
                            {
                                DataContext = vm
                            };
                            Application.Current.MainWindow = adminWindow;
                            currentWindow?.Close();
                            adminWindow.Opacity = 0;
                            adminWindow.Show();
                            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(270));
                            adminWindow.BeginAnimation(Window.OpacityProperty, fadeIn);
                        }
                        else return;
                    }
                }
            }
            catch (Exception ex)
            {
                await Toast.ShowToastAsync("Không thể kết nối đến cơ sở dữ liệu", Brushes.OrangeRed);
            }
        }
        

        [RelayCommand]
        private void ShowRegister() => _auth.NavigateToRegister();
        [RelayCommand]
        private void ShowForgotPassword() => _auth.NavigateToForgotPassword();       
    }
}
