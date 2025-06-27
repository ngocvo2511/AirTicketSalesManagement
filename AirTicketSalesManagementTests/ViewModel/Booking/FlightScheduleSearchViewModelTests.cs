using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirTicketSalesManagement.ViewModel.Booking;
using System.Collections.ObjectModel;
using AirTicketSalesManagement.Models; 

namespace AirTicketSalesManagementTests.ViewModel.Booking
{
    [TestClass]
    public class FlightScheduleSearchViewModelTests
    {
        private FlightScheduleSearchViewModel _viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _viewModel = new FlightScheduleSearchViewModel();
        }

        #region Adult Count Tests

        [TestMethod]
        public void IncreaseAdult_WhenNotAtMax()
        {
            _viewModel.AdultCount = 1;
            _viewModel.ChildCount = 0;
            _viewModel.IncreaseAdultCommand.Execute(null);

            Assert.AreEqual(2, _viewModel.AdultCount);
        }

        [TestMethod]
        public void IncreaseAdult_WhenAtMax()
        {
            _viewModel.AdultCount = 5;
            _viewModel.ChildCount = 4;
            _viewModel.IncreaseAdultCommand.Execute(null);

            Assert.AreEqual(5, _viewModel.AdultCount);
        }

        [TestMethod]
        public void DecreaseAdult_WhenAboveMin()
        {
            _viewModel.AdultCount = 2;
            _viewModel.DecreaseAdultCommand.Execute(null);

            Assert.AreEqual(1, _viewModel.AdultCount);
        }

        [TestMethod]
        public void DecreaseAdult_WhenAtMin()
        {
            _viewModel.AdultCount = 1;
            _viewModel.DecreaseAdultCommand.Execute(null);

            Assert.AreEqual(1, _viewModel.AdultCount);
        }

        [TestMethod]
        public void DecreaseAdult_WhenInfantCountExceeds()
        {
            _viewModel.AdultCount = 3;
            _viewModel.InfantCount = 3;
            _viewModel.DecreaseAdultCommand.Execute(null);

            Assert.AreEqual(2, _viewModel.AdultCount);
            Assert.AreEqual(2, _viewModel.InfantCount);
        }

        #endregion

        #region Child Count Tests

        [TestMethod]
        public void IncreaseChild_WhenNotAtMax()
        {
            _viewModel.AdultCount = 1;
            _viewModel.ChildCount = 1;
            _viewModel.IncreaseChildCommand.Execute(null);

            Assert.AreEqual(2, _viewModel.ChildCount);
        }

        [TestMethod]
        public void IncreaseChild_WhenAtMax()
        {
            _viewModel.AdultCount = 8;
            _viewModel.ChildCount = 1;
            _viewModel.IncreaseChildCommand.Execute(null);

            Assert.AreEqual(1, _viewModel.ChildCount);
        }

        [TestMethod]
        public void DecreaseChild_WhenAboveMin()
        {
            _viewModel.ChildCount = 1;
            _viewModel.DecreaseChildCommand.Execute(null);

            Assert.AreEqual(0, _viewModel.ChildCount);
        }

        #endregion

        #region Infant Count Tests

        [TestMethod]
        public void IncreaseInfant_WhenBelowAdultCount()
        {
            _viewModel.AdultCount = 2;
            _viewModel.InfantCount = 1;
            _viewModel.IncreaseInfantCommand.Execute(null);

            Assert.AreEqual(2, _viewModel.InfantCount);
        }

        [TestMethod]
        public void IncreaseInfant_WhenEqualToAdultCount()
        {
            _viewModel.AdultCount = 2;
            _viewModel.InfantCount = 2;
            _viewModel.IncreaseInfantCommand.Execute(null);

            Assert.AreEqual(2, _viewModel.InfantCount);
        }

        [TestMethod]
        public void DecreaseInfant_WhenAboveMin()
        {
            _viewModel.InfantCount = 1;
            _viewModel.DecreaseInfantCommand.Execute(null);

            Assert.AreEqual(0, _viewModel.InfantCount);
        }

        #endregion

        [TestMethod]
        public void ToggleTicketClasses()
        {
            var flight = new KQTraCuuChuyenBayMoRong { IsTicketClassesExpanded = false };
            
            _viewModel.ToggleTicketClassesCommand.Execute(flight);
            Assert.IsTrue(flight.IsTicketClassesExpanded);
            
            _viewModel.ToggleTicketClassesCommand.Execute(flight);
            Assert.IsFalse(flight.IsTicketClassesExpanded);
        }

        [TestMethod]
        public void PassengerSummary()
        {
            _viewModel.AdultCount = 2;
            _viewModel.ChildCount = 1;
            _viewModel.InfantCount = 1;
            string summary = _viewModel.PassengerSummary;

            Assert.AreEqual("4 hành khách", summary);
        }
    }
}