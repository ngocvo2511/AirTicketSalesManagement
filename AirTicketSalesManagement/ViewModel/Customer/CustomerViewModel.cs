using AirTicketSalesManagement.View.Login;
using AirTicketSalesManagement.ViewModel.Login;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace AirTicketSalesManagement.ViewModel.Customer
{
    public partial class CustomerViewModel : BaseViewModel
    {
        [ObservableProperty]
        private BaseViewModel currentViewModel;
        
        public string idCustomer {  get; set; }
        public CustomerViewModel()
        {
            CurrentViewModel = new HomePageViewModel();
        }

        [RelayCommand]
        private void NavigateToCustomerProfile()
        {
            CurrentViewModel = new CustomerProfileViewModel();
        }

        [RelayCommand]
        private void NavigateToHomePage()
        {
            CurrentViewModel = new HomePageViewModel();
        }

        [RelayCommand]
        private void NavigateToBookingHistory()
        {
            CurrentViewModel = new BookingHistoryViewModel();
        }

        [RelayCommand]
        private void NavigateToFlightScheduleSearch()
        {
            CurrentViewModel = new FlightScheduleSearchViewModel();
        }

        [RelayCommand]
        private void NavigateToFlightTicketBooking()
        {
            CurrentViewModel = new FlightScheduleSearchViewModel();
        }

        [RelayCommand]
        private void Logout()
        {
            var currentWindow = Application.Current.MainWindow;
            var authWindow = new AuthWindow();
            Application.Current.MainWindow = authWindow;
            currentWindow?.Close();
            authWindow.Opacity = 0;
            authWindow.Show();
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(270));
            authWindow.BeginAnimation(Window.OpacityProperty, fadeIn);

        }

    }
}
