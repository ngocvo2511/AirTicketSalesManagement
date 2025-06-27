using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirTicketSalesManagement.ViewModel.Customer;
using AirTicketSalesManagement.ViewModel.Booking;
using AirTicketSalesManagement.Services;
using CommunityToolkit.Mvvm.Messaging;

namespace AirTicketSalesManagementTests.ViewModel.Customer
{
    [TestClass]
    public class CustomerViewModelTests
    {
        private CustomerViewModel _viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _viewModel = new CustomerViewModel();
        }

        [TestMethod]
        public void Constructor()
        {
            Assert.IsInstanceOfType(_viewModel.CurrentViewModel, typeof(HomePageViewModel));
            Assert.IsFalse(_viewModel.IsWebViewVisible);
        }

        [TestMethod]
        public void NavigateToCustomerProfile()
        {
            _viewModel.NavigateToCustomerProfileCommand.Execute(null);
            Assert.IsInstanceOfType(_viewModel.CurrentViewModel, typeof(CustomerProfileViewModel));
        }

        [TestMethod]
        public void NavigateToBookingHistory()
        {
            _viewModel.NavigateToBookingHistoryCommand.Execute(null);
            Assert.IsInstanceOfType(_viewModel.CurrentViewModel, typeof(BookingHistoryViewModel));
        }

        [TestMethod]
        public void NavigateToFlightTicketBooking()
        {
            _viewModel.NavigateToFlightTicketBookingCommand.Execute(null);
            Assert.IsInstanceOfType(_viewModel.CurrentViewModel, typeof(FlightScheduleSearchViewModel));
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

            Assert.IsInstanceOfType(_viewModel.CurrentViewModel, typeof(BookingHistoryViewModel), "Should navigate to BookingHistoryViewModel after successful payment");
        }
    }
}