using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.ViewModel.Login
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly AuthViewModel _auth;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;
        [ObservableProperty]
        private string confirmPassword;
        [ObservableProperty]
        private string name;

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

        }

        [RelayCommand]
        private void ShowLogin() => _auth.NavigateToLogin();
    }
}
