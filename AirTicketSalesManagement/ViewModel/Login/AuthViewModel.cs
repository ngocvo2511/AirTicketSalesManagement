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
    public partial class AuthViewModel : BaseViewModel
    {
        [ObservableProperty]
        private BaseViewModel currentViewModel;

        [ObservableProperty]
        private bool isPasswordVisible;

        [ObservableProperty]
        private string password;

        public AuthViewModel()
        {
            CurrentViewModel = new LoginViewModel();
        }

        [RelayCommand]
        private void ShowRegister() => NavigateTo(new RegisterViewModel());

        [RelayCommand]
        private void ShowLogin() => NavigateTo(new LoginViewModel());

        [RelayCommand]
        private void ShowForgotPassword() => NavigateTo(new ForgotPasswordViewModel());

        [RelayCommand]
        private void CloseWindow()
        {
            Application.Current.Shutdown();
        }

        private void NavigateTo(BaseViewModel viewModel)
        {
            IsPasswordVisible = false;
            Password = string.Empty;
            CurrentViewModel = viewModel;
        }
    }
}
