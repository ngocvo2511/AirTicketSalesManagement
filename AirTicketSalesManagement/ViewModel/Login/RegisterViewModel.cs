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

namespace AirTicketSalesManagement.ViewModel.Login
{
    public partial class RegisterViewModel : BaseViewModel, INotifyDataErrorInfo, INotifyPropertyChanged
    {
        private readonly AuthViewModel _auth;
        private readonly Dictionary<string, List<string>> _errors = new();

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;
        [ObservableProperty]
        private string confirmPassword;
        [ObservableProperty]
        private string name;

        public void Validate()
        {
            ClearErrors(nameof(Email));
            ClearErrors(nameof(Password));
            ClearErrors(nameof(ConfirmPassword));
            ClearErrors(nameof(Name));
            if (string.IsNullOrWhiteSpace(Name))
                AddError(nameof(Name),"Tên không được để trống.");
            if (string.IsNullOrWhiteSpace(Email))
                AddError(nameof(Email),"Email không được để trống.");
            else if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                AddError(nameof(Email),"Email không hợp lệ.");
            
            if (string.IsNullOrWhiteSpace(Password))
            {
                AddError(nameof(Password), "Mật khẩu không được để trống.");
            }
            if (ConfirmPassword != Password)
            {
                AddError(nameof(ConfirmPassword), "Xác nhận mật khẩu không khớp với mật khẩu.");
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
        //public string this[string columnName]
        //{
        //    get
        //    {
        //        if (columnName == nameof(Name) && string.IsNullOrWhiteSpace(Name))
        //            return "Tên không được để trống.";

        //        if (columnName == nameof(Email))
        //        {
        //            if (string.IsNullOrWhiteSpace(Email))
        //                return "Email không được để trống.";
        //            if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        //                return "Email không hợp lệ.";
        //        }
        //        if (columnName == nameof(Password) && string.IsNullOrWhiteSpace(Password))
        //        {
        //            return "Mật khẩu không được để trống.";
        //        }
        //        if (columnName == nameof(ConfirmPassword) && ConfirmPassword!=Password)
        //        {
        //            return "Xác nhận mật khẩu không khớp với mật khẩu.";
        //        }
        //        return null;
        //    }
        //}
        
        public RegisterViewModel()
        {
            // Default constructor
        }

        public RegisterViewModel(AuthViewModel auth)
        {
            _auth = auth;
        }

        [RelayCommand]
        private void Register()
        {
            Validate();
            if (HasErrors) return;
            
        }

        [RelayCommand]
        private void ShowLogin() => _auth.NavigateToLogin();
    }
}
