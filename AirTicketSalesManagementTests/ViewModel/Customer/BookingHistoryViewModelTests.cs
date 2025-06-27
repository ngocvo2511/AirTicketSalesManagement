using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirTicketSalesManagement.ViewModel.Customer;
using AirTicketSalesManagement.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Reflection;

namespace AirTicketSalesManagementTests.ViewModel.Customer
{
    [TestClass]
    public class BookingHistoryViewModelTests
    {
        private BookingHistoryViewModel _viewModel;
        private ObservableCollection<KQLichSuDatVe> _mockRootData;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRootData = new ObservableCollection<KQLichSuDatVe>
            {
                new KQLichSuDatVe
                {
                    MaVe = 1, DiemDi = "Hà Nội (HAN), Vietnam", DiemDen = "TP. Hồ Chí Minh (SGN), Vietnam",
                    HangHangKhong = "Vietnam Airlines", NgayDat = new DateTime(2023, 10, 1), TrangThai = "Đã thanh toán"
                },
                new KQLichSuDatVe
                {
                    MaVe = 2, DiemDi = "TP. Hồ Chí Minh (SGN), Vietnam", DiemDen = "Đà Nẵng (DAD), Vietnam",
                    HangHangKhong = "Vietjet Air", NgayDat = new DateTime(2023, 10, 5), TrangThai = "Chờ thanh toán"
                },
                new KQLichSuDatVe
                {
                    MaVe = 3, DiemDi = "Hà Nội (HAN), Vietnam", DiemDen = "Đà Nẵng (DAD), Vietnam",
                    HangHangKhong = "Bamboo Airways", NgayDat = new DateTime(2023, 10, 10), TrangThai = "Đã hủy"
                },
                 new KQLichSuDatVe
                {
                    MaVe = 4, DiemDi = "TP. Hồ Chí Minh (SGN), Vietnam", DiemDen = "Hà Nội (HAN), Vietnam",
                    HangHangKhong = "Vietnam Airlines", NgayDat = new DateTime(2023, 10, 10), TrangThai = "Đã thanh toán"
                }
            };
            _viewModel = new BookingHistoryViewModel();

            var fieldInfo = typeof(BookingHistoryViewModel).GetField("rootHistoryBooking", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(_viewModel, _mockRootData);

            _viewModel.HistoryBooking = new ObservableCollection<KQLichSuDatVe>(_mockRootData);
        }

        [TestMethod]
        public void ClearFilter()
        {
            _viewModel.NoiDiFilter = "Hà Nội (HAN), Vietnam";
            _viewModel.HistoryBooking = new ObservableCollection<KQLichSuDatVe>(); 
            _viewModel.ClearFilterCommand.Execute(null);

            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.NoiDiFilter));
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.NoiDenFilter));
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.HangHangKhongFilter));
            Assert.IsNull(_viewModel.NgayDatFilter);
            Assert.AreEqual(_mockRootData.Count, _viewModel.HistoryBooking.Count, "HistoryBooking should be restored to the full list");
        }

        [TestMethod]
        public void SearchHistory_WithDepartureFilter()
        {
            _viewModel.NoiDiFilter = "Hà Nội (HAN), Vietnam";
            _viewModel.SearchHistoryCommand.Execute(null);

            Assert.AreEqual(2, _viewModel.HistoryBooking.Count);
            Assert.IsTrue(_viewModel.HistoryBooking.All(v => v.DiemDi == "Hà Nội (HAN), Vietnam"));
        }

        [TestMethod]
        public void SearchHistory_WithAirlineFilter()
        {
            _viewModel.HangHangKhongFilter = "Vietnam Airlines";
            _viewModel.SearchHistoryCommand.Execute(null);

            Assert.AreEqual(2, _viewModel.HistoryBooking.Count);
            Assert.IsTrue(_viewModel.HistoryBooking.All(v => v.HangHangKhong == "Vietnam Airlines"));
        }

        [TestMethod]
        public void SearchHistory_WithDateFilter()
        {
            _viewModel.NgayDatFilter = new DateTime(2023, 10, 10);
            _viewModel.SearchHistoryCommand.Execute(null);

            Assert.AreEqual(2, _viewModel.HistoryBooking.Count);
            Assert.IsTrue(_viewModel.HistoryBooking.All(v => v.NgayDat?.Date == new DateTime(2023, 10, 10).Date));
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
        public void SearchHistory_WithMultipleFilters()
        {
            _viewModel.NoiDiFilter = "TP. Hồ Chí Minh (SGN), Vietnam";
            _viewModel.HangHangKhongFilter = "Vietnam Airlines";
            _viewModel.SearchHistoryCommand.Execute(null);

            Assert.AreEqual(1, _viewModel.HistoryBooking.Count);
            var result = _viewModel.HistoryBooking.First();
            Assert.AreEqual(4, result.MaVe); 
            Assert.AreEqual("Vietnam Airlines", result.HangHangKhong);
        }

        [TestMethod]
        public void SearchHistory_WithNoResults()
        {
            _viewModel.HangHangKhongFilter = "Hãng không tồn tại";
            _viewModel.SearchHistoryCommand.Execute(null);

            Assert.AreEqual(0, _viewModel.HistoryBooking.Count);
            Assert.IsTrue(_viewModel.IsEmpty);
        }
    }
}