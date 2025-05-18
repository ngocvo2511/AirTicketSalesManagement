using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.View.Customer;
using AirTicketSalesManagement.View.Login;
using AirTicketSalesManagement.ViewModel.Customer;
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
                        if (user.VaiTro == "KhachHang")
                        {
                            var currentWindow = Application.Current.MainWindow;
                            var vm = new Customer.CustomerViewModel();
                            vm.IdCustomer = user.MaKh;

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
                        else if (user.VaiTro == "NhanVien")
                        {

                        }
                        else if (user.VaiTro == "Admin")
                        {

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
