using AirTicketSalesManagement.Services;
using AirTicketSalesManagement.View.Login;
using AirTicketSalesManagement.ViewModel.Login;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows;
using AirTicketSalesManagement.ViewModel.Booking;
using AirTicketSalesManagement.Models;
using CommunityToolkit.Mvvm.Messaging;
using AirTicketSalesManagement.ViewModel.Customer;
using AirTicketSalesManagement.ViewModel.CustomerManagement;
using AirTicketSalesManagement.Services.EmailServices;
using AirTicketSalesManagement.Messages;

namespace AirTicketSalesManagement.ViewModel.Staff
{
    public partial class StaffViewModel : BaseViewModel
    {
        [ObservableProperty]
        private BaseViewModel currentViewModel;

        [ObservableProperty]
        private string hoTen;

        [ObservableProperty]
        private bool isWebViewVisible;

        public StaffViewModel()
        {
            CurrentViewModel = new HomePageViewModel();

            //MessageBox.Show(UserSession.Current.CustomerId + " " + UserSession.Current.CustomerName);
            hoTen = UserSession.Current.CustomerName;

            WeakReferenceMessenger.Default.Register<PaymentRequestedMessage>(this, (r, m) =>
            {
                IsWebViewVisible = true;
                WeakReferenceMessenger.Default.Send(new WebViewNavigationMessage(m.Value));
            });

            WeakReferenceMessenger.Default.Register<PaymentSuccessMessage>(this, (r, m) =>
            {
                // Chuyển sang màn hình Booking History
                if (CurrentViewModel is PaymentConfirmationViewModel paymentConfirmationViewModel)
                {
                    paymentConfirmationViewModel.HandlePaymentSuccess();
                }
                CurrentViewModel = new TicketManagementViewModel(this, new EmailService(), new EmailTemplateService());
            });

            NavigationService.NavigateToAction = (viewModelType, parameter) =>
            {
                if (viewModelType == typeof(PassengerInformationViewModel))
                {
                    CurrentViewModel = new PassengerInformationViewModel((ThongTinChuyenBayDuocChon)parameter);
                }
                else if (viewModelType == typeof(PaymentConfirmationViewModel))
                {
                    CurrentViewModel = new PaymentConfirmationViewModel((ThongTinHanhKhachVaChuyenBay)parameter, new EmailService(), new EmailTemplateService());
                }
                else if (viewModelType == typeof(HomePageViewModel))
                {
                    CurrentViewModel = new Staff.HomePageViewModel();
                }
                else if (viewModelType == typeof(TicketManagementViewModel))
                {
                    CurrentViewModel = new TicketManagementViewModel(this, new EmailService(), new EmailTemplateService());
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
        }

        [RelayCommand]
        private void NavigateToStaffProfile()
        {
            WeakReferenceMessenger.Default.Send(new WebViewClearCacheMessage());
            IsWebViewVisible = false;
            CurrentViewModel = new StaffProfileViewModel();
        }

        [RelayCommand]
        private void NavigateToHomePage()
        {
            WeakReferenceMessenger.Default.Send(new WebViewClearCacheMessage());
            IsWebViewVisible = false;
            CurrentViewModel = new HomePageViewModel();
        }

        [RelayCommand]
        private void NavigateToFlightTicketBooking()
        {
            WeakReferenceMessenger.Default.Send(new WebViewClearCacheMessage());
            IsWebViewVisible = false;
            CurrentViewModel = new FlightScheduleSearchViewModel();
        }

        [RelayCommand]
        private void NavigateToTicketManagement()
        {
            WeakReferenceMessenger.Default.Send(new WebViewClearCacheMessage());
            IsWebViewVisible = false;
            CurrentViewModel = new TicketManagementViewModel(this, new EmailService(), new EmailTemplateService());
        }

        [RelayCommand]
        private void NavigateToCustomerManagement()
        {
            WeakReferenceMessenger.Default.Send(new WebViewClearCacheMessage());
            IsWebViewVisible = false;
            CurrentViewModel = new CustomerManagementViewModel();
        }

        [RelayCommand]
        private void Logout()
        {
            // Reset đầy đủ UserSession trước khi logout
            UserSession.Current.AccountId = null;
            UserSession.Current.CustomerId = null;
            UserSession.Current.StaffId = null;
            UserSession.Current.CustomerName = null;
            UserSession.Current.Email = null;
            UserSession.Current.isStaff = false;
            
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


        [ObservableProperty]
        private bool isSidebarExpanded = true;
        [ObservableProperty]
        private GridLength sidebarWidth = new GridLength(240);
        [ObservableProperty]
        private Visibility textVisibility = Visibility.Visible;
        [ObservableProperty]
        private string toggleIcon = "MenuOpen";

        [RelayCommand]
        private void ToggleSidebar()
        {
            IsSidebarExpanded = !IsSidebarExpanded;
            if (IsSidebarExpanded)
            {
                SidebarWidth = new GridLength(240);
                TextVisibility = Visibility.Visible;
                ToggleIcon = "MenuOpen";
            }
            else
            {
                SidebarWidth = new GridLength(100);
                TextVisibility = Visibility.Collapsed;
                ToggleIcon = "Menu";
            }
        }
    }
}
