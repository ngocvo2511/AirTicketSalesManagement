using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.ViewModel.Login
{
    public partial class ForgotPasswordViewModel : BaseViewModel
    {
        private readonly AuthViewModel _auth;

        [ObservableProperty]
        private string email;

        public ForgotPasswordViewModel(AuthViewModel auth)
        {
            _auth = auth;
        }

        [RelayCommand]
        private void ForgotPassword()
        {

        }

        [RelayCommand]
        private void ShowLogin() => _auth.NavigateToLogin();
    }
}
