using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirTicketSalesManagement.ViewModel.Staff;
using AirTicketSalesManagement.ViewModel.Booking;
using AirTicketSalesManagement.ViewModel.Customer;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using AirTicketSalesManagement.Services;
using AirTicketSalesManagement.ViewModel.CustomerManagement;

namespace AirTicketSalesManagementTests.ViewModel.Staff
{
    [STATestClass]
    public class StaffViewModelTests
    {
        private StaffViewModel _viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _viewModel = new StaffViewModel();
        }

        [TestMethod]
        public void Constructor()
        {
            Assert.IsInstanceOfType(_viewModel.CurrentViewModel, typeof(AirTicketSalesManagement.ViewModel.Staff.HomePageViewModel));
            Assert.IsTrue(_viewModel.IsSidebarExpanded);
            Assert.AreEqual(new GridLength(240), _viewModel.SidebarWidth);
            Assert.AreEqual(Visibility.Visible, _viewModel.TextVisibility);
            Assert.AreEqual("MenuOpen", _viewModel.ToggleIcon);
        }

        [TestMethod]
        public void NavigateToStaffProfile()
        {
            _viewModel.NavigateToStaffProfileCommand.Execute(null);
            Assert.IsInstanceOfType(_viewModel.CurrentViewModel, typeof(StaffProfileViewModel));
        }

        [TestMethod]
        public void NavigateToFlightTicketBooking()
        {
            _viewModel.NavigateToFlightTicketBookingCommand.Execute(null);
            Assert.IsInstanceOfType(_viewModel.CurrentViewModel, typeof(FlightScheduleSearchViewModel));
        }

        [TestMethod]
        public void NavigateToTicketManagement()
        {
            _viewModel.NavigateToTicketManagementCommand.Execute(null);
            Assert.IsInstanceOfType(_viewModel.CurrentViewModel, typeof(TicketManagementViewModel));
        }

        [TestMethod]
        public void NavigateToCustomerManagement()
        {
            _viewModel.NavigateToCustomerManagementCommand.Execute(null);
            Assert.IsInstanceOfType(_viewModel.CurrentViewModel, typeof(CustomerManagementViewModel));
        }

        [TestMethod]
        public void ToggleSidebar_CollapsesSidebar()
        {
            _viewModel.IsSidebarExpanded = true;
            _viewModel.ToggleSidebarCommand.Execute(null);

            Assert.IsFalse(_viewModel.IsSidebarExpanded);
            Assert.AreEqual(new GridLength(100), _viewModel.SidebarWidth);
            Assert.AreEqual(Visibility.Collapsed, _viewModel.TextVisibility);
            Assert.AreEqual("Menu", _viewModel.ToggleIcon);
        }

        [TestMethod]
        public void ToggleSidebar_ExpandsSidebar()
        {
            _viewModel.IsSidebarExpanded = false;
            _viewModel.SidebarWidth = new GridLength(100);
            _viewModel.TextVisibility = Visibility.Collapsed;
            _viewModel.ToggleIcon = "Menu";
            _viewModel.ToggleSidebarCommand.Execute(null);

            Assert.IsTrue(_viewModel.IsSidebarExpanded);
            Assert.AreEqual(new GridLength(240), _viewModel.SidebarWidth);
            Assert.AreEqual(Visibility.Visible, _viewModel.TextVisibility);
            Assert.AreEqual("MenuOpen", _viewModel.ToggleIcon);
        }

        [TestMethod]
        public void ReceivingPaymentRequestedMessage()
        {
            _viewModel.IsWebViewVisible = false;
            var message = new PaymentRequestedMessage("https://vnpay.vn/test-url-staff");
            WeakReferenceMessenger.Default.Send(message);

            Assert.IsTrue(_viewModel.IsWebViewVisible, "WebView should become visible after receiving a PaymentRequestedMessage");
        }

        [TestMethod]
        public void ReceivingPaymentSuccessMessage()
        {
            var mockPaymentVM = new PaymentConfirmationViewModel(new AirTicketSalesManagement.Models.ThongTinHanhKhachVaChuyenBay
            {
                FlightInfo = new AirTicketSalesManagement.Models.ThongTinChuyenBayDuocChon
                {
                    Flight = new AirTicketSalesManagement.Models.KQTraCuuChuyenBayMoRong(),
                    TicketClass = new AirTicketSalesManagement.Models.HangVe()
                }
            });
            _viewModel.CurrentViewModel = mockPaymentVM;
            var message = new PaymentSuccessMessage();
            WeakReferenceMessenger.Default.Send(message);

            Assert.IsInstanceOfType(_viewModel.CurrentViewModel, typeof(TicketManagementViewModel), "Should navigate to TicketManagementViewModel after successful payment");
        }
    }
}