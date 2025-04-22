using AirTicketSalesManagement.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace AirTicketSalesManagement.ViewModel.Login
{
    public partial class LoginViewModel : BaseViewModel, INotifyDataErrorInfo, INotifyPropertyChanged
    {
        private readonly AuthViewModel _auth;
        private readonly Dictionary<string, List<string>> _errors = new();

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isPasswordVisible;


        public void Validate()
        {
            ClearErrors(nameof(Email));

            if (string.IsNullOrWhiteSpace(Email) || !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$") || string.IsNullOrWhiteSpace(Password))
            {
                AddError(nameof(Email), "Tài khoản hoặc mật khẩu không hợp lệ");
                return;
            }
            using (var context = new AirTicketDbContext())
            {
                var user = context.Taikhoans.FirstOrDefault(x => x.Email == Email);
                if (user == null)
                {
                    AddError(nameof(Email), "Tài khoản hoặc mật khẩu không hợp lệ");
                    return;
                }
                else
                {
                    if(!BCrypt.Net.BCrypt.Verify(Password, user.MatKhau))
                    {
                        AddError(nameof(Email), "Tài khoản hoặc mật khẩu không hợp lệ");
                        return;
                    }
                }
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public LoginViewModel()
        {
            // Default constructor
        }
        public LoginViewModel(AuthViewModel auth)
        {
            _auth = auth;
        }

        [RelayCommand]
        private void Login()
        {
            Validate();
            if (HasErrors) return;

        }
        

        [RelayCommand]
        private void ShowRegister() => _auth.NavigateToRegister();
        [RelayCommand]
        private void ShowForgotPassword() => _auth.NavigateToForgotPassword();       
    }
}
