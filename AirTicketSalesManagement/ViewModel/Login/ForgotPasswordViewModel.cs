using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Services.EmailServices;
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
using System.Windows.Media;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AirTicketSalesManagement.ViewModel.Login
{
    public partial class ForgotPasswordViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        private readonly AuthViewModel _auth;
        private readonly Dictionary<string, List<string>> _errors = new();
        public ToastViewModel Toast { get; } = new ToastViewModel();
        [ObservableProperty]
        private string email;

        public ForgotPasswordViewModel(){}

        public ForgotPasswordViewModel(AuthViewModel auth)
        {
            _auth = auth;
        }

        [RelayCommand]
        private async Task ForgotPasswordAsync()
        {
            Validate();
            if (HasErrors)
            {
                return;
            }
            try
            {
                using(var context = new AirTicketDbContext())
                {
                    var user = context.Taikhoans.FirstOrDefault(x => x.Email == Email);
                    if(user == null)
                    {
                        AddError(nameof(Email), "Tài khoản không tồn tại");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                await Toast.ShowToastAsync("Không thể kết nối đến cơ sở dữ liệu", Brushes.OrangeRed);
                return;
            }
            _auth.CurrentViewModel = new ResetPasswordViewModel(_auth, Email, new EmailService(), new OtpService(), new EmailTemplateService());
        }

        [RelayCommand]
        private void ShowLogin() => _auth.NavigateToLogin();

        #region Error
        public void Validate()
        {
            ClearErrors(nameof(Email));

            if (string.IsNullOrWhiteSpace(Email) || !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                AddError(nameof(Email), "Email không hợp lệ");
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
    }
}
