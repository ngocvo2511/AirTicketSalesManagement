using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.Services;
using AirTicketSalesManagement.View.Login;
using AirTicketSalesManagement.ViewModel.Booking;
using AirTicketSalesManagement.ViewModel.Login;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
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

        [ObservableProperty]
        private string hoTen;

        [ObservableProperty]
        private bool isWebViewVisible;

        public int? IdCustomer { get; set; }
        public CustomerViewModel()
        {
            CurrentViewModel = new HomePageViewModel();

            //MessageBox.Show(UserSession.Current.CustomerId + " " + UserSession.Current.CustomerName);
            hoTen = UserSession.Current.CustomerName;

            WeakReferenceMessenger.Default.Register<PaymentRequestedMessage>(this, (r, m) =>
            {
                IsWebViewVisible = true;
                WeakReferenceMessenger.Default.Send(new WebViewNavigationMessage(m.Value));
            });


            NavigationService.NavigateToAction = (viewModelType, parameter) =>
            {
                if (viewModelType == typeof(PassengerInformationViewModel))
                {
                    CurrentViewModel = new PassengerInformationViewModel((ThongTinChuyenBayDuocChon)parameter);
                }
                else if (viewModelType == typeof(PaymentConfirmationViewModel))
                {
                    CurrentViewModel = new PaymentConfirmationViewModel((ThongTinHanhKhachVaChuyenBay)parameter);
                }
                else if (viewModelType == typeof(HomePageViewModel))
                {
                    CurrentViewModel = new HomePageViewModel();
                }
            };

            NavigationService.NavigateBackAction = (previousViewModelType, previousParameter) =>
            {
                if (previousViewModelType == typeof(PassengerInformationViewModel))
                {
                    CurrentViewModel = new FlightScheduleSearchViewModel();
                }
                else if (previousViewModelType == typeof(PaymentConfirmationViewModel))
                {
                    CurrentViewModel = new PassengerInformationViewModel((ThongTinHanhKhachVaChuyenBay)previousParameter);
                }
            };

            WeakReferenceMessenger.Default.Register<PaymentSuccessMessage>(this, (r, m) =>
            {
                // Chuyển sang màn hình Booking History
                if (CurrentViewModel is PaymentConfirmationViewModel paymentConfirmationViewModel)
                {
                    paymentConfirmationViewModel.HandlePaymentSuccess();
                }
                CurrentViewModel = new BookingHistoryViewModel(IdCustomer, this);
            });
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
            CurrentViewModel = new BookingHistoryViewModel(IdCustomer, this);
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

    public class WebViewNavigationMessage : ValueChangedMessage<string>
    {
        public WebViewNavigationMessage(string url) : base(url) { }
    }
}
