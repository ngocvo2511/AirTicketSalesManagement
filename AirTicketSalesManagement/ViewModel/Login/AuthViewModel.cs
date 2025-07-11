using AirTicketSalesManagement.Services.EmailServices;
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

        public AuthViewModel()
        {
            CurrentViewModel = new LoginViewModel(this);
        }

        
        [RelayCommand]
        private void CloseWindow()
        {
            Application.Current.Shutdown();
        }

        public void NavigateToRegister()
        {
            CurrentViewModel = new RegisterViewModel(this, new EmailService(), new OtpService(), new EmailTemplateService());
        }

        public void NavigateToLogin()
        {
            CurrentViewModel = new LoginViewModel(this);
        }

        public void NavigateToForgotPassword()
        {
            CurrentViewModel = new ForgotPasswordViewModel(this);
        }
    }
}
