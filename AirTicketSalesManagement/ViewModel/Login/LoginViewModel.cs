using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AirTicketSalesManagement.ViewModel.Login
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly AuthViewModel _auth;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isPasswordVisible;

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
            
        }

        [RelayCommand]
        private void ShowRegister() => _auth.NavigateToRegister();
        [RelayCommand]
        private void ShowForgotPassword() => _auth.NavigateToForgotPassword();
    }
}
