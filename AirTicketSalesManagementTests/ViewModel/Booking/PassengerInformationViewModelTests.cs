using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirTicketSalesManagement.ViewModel.Booking;
using AirTicketSalesManagement.Models;
using System.Linq;

namespace AirTicketSalesManagementTests.ViewModel.Booking
{
    [TestClass]
    public class PassengerInformationViewModelTests
    {
        private ThongTinChuyenBayDuocChon _mockSelectedFlightInfo;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockSelectedFlightInfo = new ThongTinChuyenBayDuocChon
            {
                Flight = new KQTraCuuChuyenBayMoRong
                {
                    MaSBDi = "SGN",
                    MaSBDen = "HAN",
                    HangHangKhong = "Vietnam Airlines",
                    NumberAdults = 2,
                    NumberChildren = 1,
                    NumberInfants = 1
                },
                TicketClass = new HangVe
                {
                    TenHangVe = "Phổ thông"
                }
            };
        }

        [TestMethod]
        public void Constructor()
        {
            var viewModel = new PassengerInformationViewModel(_mockSelectedFlightInfo);
            Assert.IsNotNull(viewModel.PassengerList, "PassengerList should not be null.");
            Assert.AreEqual("SGN - HAN (Vietnam Airlines)", viewModel.FlightCode, "FlightCode is not formatted correctly.");
            Assert.AreEqual("Phổ thông", viewModel.SelectedTicketClass.TenHangVe, "SelectedTicketClass is not set correctly.");

            int expectedTotal = _mockSelectedFlightInfo.Flight.NumberAdults +
                                _mockSelectedFlightInfo.Flight.NumberChildren +
                                _mockSelectedFlightInfo.Flight.NumberInfants;
            Assert.AreEqual(expectedTotal, viewModel.PassengerList.Count, "Total number of passengers should be correct.");

            int actualAdults = viewModel.PassengerList.Count(p => p.PassengerType == PassengerType.Adult);
            int actualChildren = viewModel.PassengerList.Count(p => p.PassengerType == PassengerType.Child);
            int actualInfants = viewModel.PassengerList.Count(p => p.PassengerType == PassengerType.Infant);

            Assert.AreEqual(_mockSelectedFlightInfo.Flight.NumberAdults, actualAdults, "Number of adults is incorrect.");
            Assert.AreEqual(_mockSelectedFlightInfo.Flight.NumberChildren, actualChildren, "Number of children is incorrect.");
            Assert.AreEqual(_mockSelectedFlightInfo.Flight.NumberInfants, actualInfants, "Number of infants is incorrect.");
        }

        [TestMethod]
        public void PassengerInfoModel()
        {
            var adult = new PassengerInfoModel { PassengerType = PassengerType.Adult };
            var child = new PassengerInfoModel { PassengerType = PassengerType.Child };
            var infant = new PassengerInfoModel { PassengerType = PassengerType.Infant };

            Assert.IsTrue(adult.IsAdult);
            Assert.IsFalse(child.IsAdult);

            Assert.AreEqual("Người lớn", adult.PassengerTypeText);
            Assert.AreEqual("Trẻ em", child.PassengerTypeText);
            Assert.AreEqual("Em bé", infant.PassengerTypeText);
        }

        [TestMethod]
        public void InfantModel()
        {
            var viewModel = new PassengerInformationViewModel(_mockSelectedFlightInfo);
            var infantModel = viewModel.PassengerList.FirstOrDefault(p => p.PassengerType == PassengerType.Infant);

            Assert.IsNotNull(infantModel, "Infant model should exist.");
            Assert.IsNotNull(infantModel.AdultPassengers, "AdultPassengers list should not be null for an infant.");
            Assert.AreEqual(_mockSelectedFlightInfo.Flight.NumberAdults, infantModel.AdultPassengers.Count);
        }
    }
}