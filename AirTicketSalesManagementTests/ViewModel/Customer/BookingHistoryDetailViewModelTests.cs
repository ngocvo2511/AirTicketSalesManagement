using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirTicketSalesManagement.ViewModel.Customer;
using AirTicketSalesManagement.Models;
using System.Collections.ObjectModel;

namespace AirTicketSalesManagementTests.ViewModel.Customer
{
    [TestClass]
    public class BookingHistoryDetailViewModelTests
    {
        private CustomerViewModel _mockParentViewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockParentViewModel = new CustomerViewModel();
        }

        [TestMethod]
        public void Constructor_WithValidData_InitializesPropertiesCorrectly()
        {
            var mockBookingHistory = new KQLichSuDatVe
            {
                MaVe = 101,
                HangHangKhong = "Vietnam Airlines",
                TrangThai = "Đã thanh toán"
            };

            var viewModel = new BookingHistoryDetailViewModel(mockBookingHistory, _mockParentViewModel);

            Assert.IsNotNull(viewModel.LichSuDatVe, "LichSuDatVe property should be set");
            Assert.AreEqual(101, viewModel.LichSuDatVe.MaVe);
            Assert.AreEqual("Vietnam Airlines", viewModel.LichSuDatVe.HangHangKhong);
            Assert.IsNotNull(viewModel.CtdvList, "CtdvList should be initialized");
        }

        [TestMethod]
        [DataRow("Đã thanh toán", true)] 
        [DataRow("Chờ thanh toán", true)]
        [DataRow("Đã hủy", false)] 
        public void Constructor_SetsCanCancelPropertyBasedOnBookingStatus(string status, bool expectedCanCancel)
        {
            var mockBookingHistory = new KQLichSuDatVe
            {
                MaVe = 102,
                TrangThai = status,
                QdHuyVe = 1,
                NgayDat = DateTime.Now.AddDays(-1), // Past date to allow cancellation
                GioDi = DateTime.Now.AddDays(2), // Future date to allow cancellation
            };
            var viewModel = new BookingHistoryDetailViewModel(mockBookingHistory, _mockParentViewModel);

            Assert.AreEqual(expectedCanCancel, viewModel.LichSuDatVe.CanCancel, $"Failed for status: {status}");
        }

        [TestMethod]
        public void GoBack()
        {
            var mockBookingHistory = new KQLichSuDatVe();
            var viewModel = new BookingHistoryDetailViewModel(mockBookingHistory, _mockParentViewModel);
            var initialViewModel = _mockParentViewModel.CurrentViewModel;
            viewModel.GoBackCommand.Execute(null);

            Assert.IsInstanceOfType(_mockParentViewModel.CurrentViewModel, typeof(BookingHistoryViewModel));
            Assert.AreNotSame(initialViewModel, _mockParentViewModel.CurrentViewModel);
        }
    }
}