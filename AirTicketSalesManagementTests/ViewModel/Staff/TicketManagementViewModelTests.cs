using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirTicketSalesManagement.ViewModel.Staff;
using AirTicketSalesManagement.Models.UIModels;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Reflection;

namespace AirTicketSalesManagementTests.ViewModel.Staff
{
    [STATestClass]
    public class TicketManagementViewModelTests
    {
        private TicketManagementViewModel _viewModel;
        private ObservableCollection<QuanLiDatVe> _mockRootData;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRootData = new ObservableCollection<QuanLiDatVe>
            {
                new QuanLiDatVe
                {
                    MaVe = 1, DiemDi = "Hà Nội (HAN), Vietnam", DiemDen = "TP. Hồ Chí Minh (SGN), Vietnam",
                    HangHangKhong = "Vietnam Airlines", NgayDat = new DateTime(2023, 10, 1), TrangThai = "Đã thanh toán",
                    EmailNguoiDat = "user1@example.com"
                },
                new QuanLiDatVe
                {
                    MaVe = 2, DiemDi = "TP. Hồ Chí Minh (SGN), Vietnam", DiemDen = "Đà Nẵng (DAD), Vietnam",
                    HangHangKhong = "Vietjet Air", NgayDat = new DateTime(2023, 10, 5), TrangThai = "Chờ thanh toán (Online)",
                    EmailNguoiDat = "user2@example.com"
                },
                new QuanLiDatVe
                {
                    MaVe = 3, DiemDi = "Hà Nội (HAN), Vietnam", DiemDen = "Đà Nẵng (DAD), Vietnam",
                    HangHangKhong = "Bamboo Airways", NgayDat = new DateTime(2023, 10, 10), TrangThai = "Đã hủy",
                    EmailNguoiDat = "user1@example.com"
                }
            };

            var mockParentViewModel = new StaffViewModel();
            _viewModel = new TicketManagementViewModel(mockParentViewModel);

            var fieldInfo = typeof(TicketManagementViewModel).GetField("rootHistoryBooking", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(_viewModel, _mockRootData);

            _viewModel.HistoryBooking = new ObservableCollection<QuanLiDatVe>(_mockRootData);
        }

        [TestMethod]
        public void ClearFilter()
        {
            _viewModel.NoiDiFilter = "Hà Nội (HAN), Vietnam";
            _viewModel.EmailFilter = "user1@example.com";
            _viewModel.HistoryBooking.Clear();
            _viewModel.ClearFilter();

            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.NoiDiFilter));
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.EmailFilter));
            Assert.AreEqual("Tất cả", _viewModel.BookingStatusFilter); 
            Assert.AreEqual(_mockRootData.Count, _viewModel.HistoryBooking.Count, "HistoryBooking should be restored to the full list");
        }

        [TestMethod]
        public void SearchHistory_WithEmailFilter()
        {
            _viewModel.EmailFilter = "user1@example.com";
            _viewModel.SearchHistoryCommand.Execute(null);

            Assert.AreEqual(2, _viewModel.HistoryBooking.Count);
            Assert.IsTrue(_viewModel.HistoryBooking.All(v => v.EmailNguoiDat == "user1@example.com"));
        }

        [TestMethod]
        public void SearchHistory_WithCaseInsensitiveEmailFilter()
        {
            _viewModel.EmailFilter = "USER1@EXAMPLE.COM";
            _viewModel.SearchHistoryCommand.Execute(null);

            Assert.AreEqual(2, _viewModel.HistoryBooking.Count);
            Assert.IsTrue(_viewModel.HistoryBooking.All(v => v.EmailNguoiDat.Equals("user1@example.com", StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public void SearchHistory_WithStatusFilter()
        {
            _viewModel.BookingStatusFilter = "Đã hủy";
            _viewModel.SearchHistoryCommand.Execute(null);

            Assert.AreEqual(1, _viewModel.HistoryBooking.Count);
            Assert.AreEqual("Đã hủy", _viewModel.HistoryBooking.First().TrangThai);
        }

        [TestMethod]
        public void SearchHistory_WithDepartureAndStatusFilter()
        {
            _viewModel.NoiDiFilter = "Hà Nội (HAN), Vietnam";
            _viewModel.BookingStatusFilter = "Đã thanh toán";
            _viewModel.SearchHistoryCommand.Execute(null);

            Assert.AreEqual(1, _viewModel.HistoryBooking.Count);
            var result = _viewModel.HistoryBooking.First();
            Assert.AreEqual(1, result.MaVe);
            Assert.AreEqual("Đã thanh toán", result.TrangThai);
        }

        [TestMethod]
        public void SearchHistory_WithNoResults()
        {
            _viewModel.EmailFilter = "nonexistent@user.com";
            _viewModel.SearchHistoryCommand.Execute(null);

            Assert.AreEqual(0, _viewModel.HistoryBooking.Count);
            Assert.IsTrue(_viewModel.IsEmpty);
        }
    }
}