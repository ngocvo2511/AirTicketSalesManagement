using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.Services;
using AirTicketSalesManagement.Services.EmailServices;
using AirTicketSalesManagement.View.Login;
using AirTicketSalesManagement.ViewModel.Booking;
using AirTicketSalesManagement.ViewModel.Login;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Windows;
using System.Windows.Media.Animation;
using AirTicketSalesManagement.Messages;

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
                    CurrentViewModel = new PaymentConfirmationViewModel((ThongTinHanhKhachVaChuyenBay)parameter, new EmailService(), new EmailTemplateService());
                }
                else if (viewModelType == typeof(HomePageViewModel))
                {
                    CurrentViewModel = new HomePageViewModel();
                }
                else if (viewModelType == typeof(BookingHistoryViewModel))
                {
                    CurrentViewModel = new BookingHistoryViewModel(IdCustomer, this, new EmailService(), new EmailTemplateService());
                }
                else if (viewModelType == typeof(FlightScheduleSearchViewModel))
                {
                    if (parameter is SearchFlightParameters searchParams)
                    {
                        CurrentViewModel = new FlightScheduleSearchViewModel(searchParams);
                    }
                    else
                    {
                        CurrentViewModel = new FlightScheduleSearchViewModel();
                    }
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
                    CurrentViewModel = new BookingHistoryViewModel(IdCustomer, this, new EmailService(), new EmailTemplateService());
                }
                else if (CurrentViewModel is BookingHistoryViewModel bookingHistoryViewModel)
                {
                    bookingHistoryViewModel.HandlePaymentSuccess();
                }
            });
        }


        [RelayCommand]
        private void NavigateToCustomerProfile()
        {
            WeakReferenceMessenger.Default.Send(new WebViewClearCacheMessage());
            IsWebViewVisible = false;
            CurrentViewModel = new CustomerProfileViewModel();
        }

        [RelayCommand]
        private void NavigateToHomePage()
        {
            WeakReferenceMessenger.Default.Send(new WebViewClearCacheMessage());
            IsWebViewVisible = false;
            CurrentViewModel = new HomePageViewModel();
        }

        [RelayCommand]
        private void NavigateToBookingHistory()
        {
            WeakReferenceMessenger.Default.Send(new WebViewClearCacheMessage());
            IsWebViewVisible = false;
            CurrentViewModel = new BookingHistoryViewModel(IdCustomer, this, new EmailService(), new EmailTemplateService());
        }

        [RelayCommand]
        private void NavigateToFlightTicketBooking()
        {
            WeakReferenceMessenger.Default.Send(new WebViewClearCacheMessage());
            IsWebViewVisible = false;
            CurrentViewModel = new FlightScheduleSearchViewModel();
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

    public class WebViewNavigationMessage : ValueChangedMessage<string>
    {
        public WebViewNavigationMessage(string url) : base(url) { }
    }
}
