using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.Services;
using AirTicketSalesManagement.ViewModel.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AirTicketSalesManagement.View.Customer
{
    /// <summary>
    /// Interaction logic for CustomerWindow.xaml
    /// </summary>
    public partial class CustomerWindow : Window
    {
        public CustomerWindow()
        {
            InitializeComponent();

            NavigationService.NavigateToAction = (viewModelType, parameter) =>
            {
                if (viewModelType == typeof(PassengerInformationViewModel))
                {
                    var viewModel = new PassengerInformationViewModel((ThongTinChuyenBayDuocChon)parameter);
                    var view = new PassengerInformationView { DataContext = viewModel };
                    MainContent.Content = view;
                }
                else if (viewModelType == typeof(FlightScheduleSearchViewModel))
                {
                    var viewModel = new FlightScheduleSearchViewModel();
                    var view = new FlightScheduleSearchView { DataContext = viewModel };
                    MainContent.Content = view;
                }
            };

            NavigationService.NavigateBackAction = () =>
            {
                // Quay lại View trước đó
                var parameter = NavigationService.GetCurrentParameter();
                if (parameter is ThongTinChuyenBayDuocChon selectedFlightInfo)
                {
                    var viewModel = new FlightScheduleSearchViewModel();
                    var view = new FlightScheduleSearchView { DataContext = viewModel };
                    MainContent.Content = view;
                }
            };

        }
    }
}
