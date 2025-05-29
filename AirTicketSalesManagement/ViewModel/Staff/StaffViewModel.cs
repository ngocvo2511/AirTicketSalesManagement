using AirTicketSalesManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.ViewModel.Staff
{
    public partial class StaffViewModel : BaseViewModel
    {
        [ObservableProperty]
        private BaseViewModel currentViewModel;

        [ObservableProperty]
        private string hoTen;

        public StaffViewModel()
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

    }
}
