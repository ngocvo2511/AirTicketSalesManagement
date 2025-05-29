using AirTicketSalesManagement.Services;
using AirTicketSalesManagement.View.Login;
using AirTicketSalesManagement.ViewModel.Booking;
using AirTicketSalesManagement.ViewModel.Login;
using AirTicketSalesManagement.ViewModel.Staff;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows;

namespace AirTicketSalesManagement.ViewModel.Admin
{
    public partial class AdminViewModel : BaseViewModel
    {

        [ObservableProperty]
        private BaseViewModel currentViewModel;

        [ObservableProperty]
        private string hoTen;

        public AdminViewModel()
        {
            CurrentViewModel = new HomePageViewModel();

            //MessageBox.Show(UserSession.Current.CustomerId + " " + UserSession.Current.CustomerName);
            hoTen = UserSession.Current.CustomerName;

            
        }

        [RelayCommand]
        private void NavigateToStaffProfile()
        {
            CurrentViewModel = new StaffProfileViewModel();
        }

        [RelayCommand]
        private void NavigateToHomePage()
        {
            CurrentViewModel = new HomePageViewModel();
        }

        [RelayCommand]
        private void NavigateToFlightTicketBooking()
        {
            CurrentViewModel = new FlightScheduleSearchViewModel();
        }

        [RelayCommand]
        private void NavigateToTicketManagement()
        {
            CurrentViewModel = new TicketManagementViewModel();
        }

        [RelayCommand]
        private void NavigateToCustomerManagement()
        {
            CurrentViewModel = new CustomerManagementViewModel();
        }

        [RelayCommand]
        private void Logout()
        {
            UserSession.Current.CustomerId = null;
            UserSession.Current.CustomerName = null;
            UserSession.Current.StaffId = null;
            var currentWindow = Application.Current.MainWindow;
            var authWindow = new AuthWindow()
            {
                DataContext = new AuthViewModel()
            };
            Application.Current.MainWindow = authWindow;
            currentWindow?.Close();
            authWindow.Opacity = 0;
            authWindow.Show();
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(270));
            authWindow.BeginAnimation(Window.OpacityProperty, fadeIn);
        }
    }
}
