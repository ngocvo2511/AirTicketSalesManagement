using AirTicketSalesManagement.Services;
using AirTicketSalesManagement.View.Login;
using AirTicketSalesManagement.ViewModel.Login;
using AirTicketSalesManagement.ViewModel.Staff;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Media.Animation;
using System.Windows;
using AirTicketSalesManagement.ViewModel.CustomerManagement;
using AirTicketSalesManagement.Services.EmailServices;

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
        private void NavigateToAdminProfile()
        {
            CurrentViewModel = new AdminProfileViewModel();
        }

        [RelayCommand]
        private void NavigateToHomePage()
        {
            CurrentViewModel = new HomePageViewModel();
        }

        [RelayCommand]
        private void NavigateToTicketManagement()
        {
            CurrentViewModel = new TicketManagementViewModel(this, new EmailService(), new EmailTemplateService());
        }

        [RelayCommand]
        private void NavigateToCustomerManagement()
        {
            CurrentViewModel = new CustomerManagementViewModel();
        }

        [RelayCommand]
        private void NavigateToReport()
        {
            CurrentViewModel = new ReportViewModel();
        }

        [RelayCommand]
        private void NavigateToFlightManagement()
        {
            CurrentViewModel = new FlightManagementViewModel();
        }

        [RelayCommand]
        private void NavigateToScheduleManagement()
        {
            CurrentViewModel = new ScheduleManagementViewModel();
        }

        [RelayCommand]
        private void NavigateToAccountManagement()
        {
            CurrentViewModel = new AccountManagementViewModel();
        }

        [RelayCommand]
        private void NavigateToRegulationManagement()
        {
            CurrentViewModel = new RegulationManagementViewModel();
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
                SidebarWidth = new GridLength(120);
                TextVisibility = Visibility.Collapsed;
                ToggleIcon = "Menu";
            }
        }
    }
}
