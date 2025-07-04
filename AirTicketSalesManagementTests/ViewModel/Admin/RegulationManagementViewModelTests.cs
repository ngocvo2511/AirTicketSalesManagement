using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirTicketSalesManagement.ViewModel.Admin;

namespace AirTicketSalesManagementTests.ViewModel.Admin
{
    [TestClass]
    public class RegulationManagementViewModelTests
    {
        private RegulationManagementViewModel _viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _viewModel = new RegulationManagementViewModel();
        }

        [TestMethod]
        public void EditMaxAirports()
        {
            _viewModel.MaxAirports = 10;
            _viewModel.IsEditingMaxAirports = false;
            _viewModel._EditMaxAirportsCommand.Execute(null);

            Assert.IsTrue(_viewModel.IsEditingMaxAirports, "Editing mode for MaxAirports should be enabled");
            Assert.AreEqual(_viewModel.MaxAirports, _viewModel.EditMaxAirports, "Value should be copied to the edit property");
        }


        [TestMethod]
        public void CancelMaxAirports()
        {
            _viewModel.MaxAirports = 10;
            _viewModel.IsEditingMaxAirports = true;
            _viewModel.EditMaxAirports = 99; 
            _viewModel.CancelMaxAirportsCommand.Execute(null);

            Assert.IsFalse(_viewModel.IsEditingMaxAirports, "Editing mode for MaxAirports should be disabled");
            Assert.AreEqual(10, _viewModel.MaxAirports, "The main value should not have changed");
        }

        [TestMethod]
        public void EditMinFlightTime()
        {
            _viewModel.MinFlightTime = 30;
            _viewModel.IsEditingMinFlightTime = false;
            _viewModel._EditMinFlightTimeCommand.Execute(null);

            Assert.IsTrue(_viewModel.IsEditingMinFlightTime);
            Assert.AreEqual(30, _viewModel.EditMinFlightTime);
        }


        [TestMethod]
        public void CancelBookingTime()
        {
            _viewModel.BookingTime = 24;
            _viewModel.IsEditingBookingTime = true;
            _viewModel.EditBookingTime = 48;
            _viewModel.CancelBookingTimeCommand.Execute(null);

            Assert.IsFalse(_viewModel.IsEditingBookingTime);
            Assert.AreEqual(24, _viewModel.BookingTime); 
        }

        [TestMethod]
        public void EditCancelTime()
        {
            _viewModel.CancelTime = 12;
            _viewModel._EditCancelTimeCommand.Execute(null);

            Assert.IsTrue(_viewModel.IsEditingCancelTime);
            Assert.AreEqual(12, _viewModel.EditCancelTime);
        }
    }
}