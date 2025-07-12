using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.ViewModel.Admin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace AirTicketSalesManagementTests.ViewModel.Admin
{
    [TestClass]
    public class FlightManagementViewModelTests
    {
        private FlightManagementViewModel _viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _viewModel = new FlightManagementViewModel();
        }

        [TestMethod]
        public void ClearSearch()
        {
            _viewModel.DiemDi = "Hà Nội, Vietnam";
            _viewModel.DiemDen = "TP. Hồ Chí Minh, Vietnam";
            _viewModel.SoHieuCB = "VN123";
            _viewModel.TrangThai = "Đang khai thác";
            _viewModel.HangHangKhong = "Vietnam Airlines";
            _viewModel.ClearSearchCommand.Execute(null);

            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.DiemDi));
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.DiemDen));
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.SoHieuCB));
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.TrangThai));
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.HangHangKhong));
        }

        [TestMethod]
        public void AddFlight()
        {
            _viewModel.AddSoHieuCB = "heheflight";
            _viewModel.DanhSachSBTG = new ObservableCollection<SBTG> { new SBTG() };
            _viewModel.AddFlightCommand.Execute(null);

            Assert.IsTrue(_viewModel.IsAddPopupOpen);
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.AddSoHieuCB));
            Assert.IsNotNull(_viewModel.DanhSachSBTG);
            Assert.AreEqual(0, _viewModel.DanhSachSBTG.Count, "DanhSachSBTG should be cleared");
        }

        [TestMethod]
        public void CancelAddFlight()
        {
            _viewModel.IsAddPopupOpen = true;
            _viewModel.CancelAddFlightCommand.Execute(null);

            Assert.IsFalse(_viewModel.IsAddPopupOpen);
        }

        [TestMethod]
        public void EditFlight_WithSelectedFlight()
        {
            _viewModel.SelectedFlight = new Chuyenbay { SoHieuCb = "VN249" };
            _viewModel.EditFlightCommand.Execute(null);

            Assert.IsTrue(_viewModel.IsEditPopupOpen);
            Assert.AreEqual("VN249", _viewModel.EditSoHieuCB);
        }

        [TestMethod]
        public void CancelEditFlight_ClosesEditPopup()
        {
            _viewModel.IsEditPopupOpen = true;
            _viewModel.CancelEditFlightCommand.Execute(null);

            Assert.IsFalse(_viewModel.IsEditPopupOpen);
        }

        [TestMethod]
        public void AddIntermediateAirport()
        {
            _viewModel.DanhSachSBTG = new ObservableCollection<SBTG>();
            _viewModel.AddIntermediateAirportCommand.Execute(null);

            Assert.AreEqual(1, _viewModel.DanhSachSBTG.Count);
            Assert.AreEqual(1, _viewModel.DanhSachSBTG.First().STT); 
        }

        //[TestMethod]
        //public void RemoveIntermediateAirport()
        //{
        //    _viewModel.DanhSachSBTG = new ObservableCollection<SBTG>
        //    {
        //        new SBTG { STT = 1, MaSBTG = "Sân bay A" },
        //        new SBTG { STT = 2, MaSBTG = "Sân bay B" },
        //        new SBTG { STT = 3, MaSBTG = "Sân bay C" }
        //    };
        //    var sbtgToRemove = _viewModel.DanhSachSBTG[1];
        //    _viewModel.RemoveIntermediateAirport(sbtgToRemove);

        //    Assert.AreEqual(2, _viewModel.DanhSachSBTG.Count); 
        //    Assert.IsNull(_viewModel.DanhSachSBTG.FirstOrDefault(s => s.MaSBTG == "Sân bay B")); 
        //    Assert.AreEqual(1, _viewModel.DanhSachSBTG.FirstOrDefault(s => s.MaSBTG == "Sân bay A").STT);
        //    Assert.AreEqual(2, _viewModel.DanhSachSBTG.FirstOrDefault(s => s.MaSBTG == "Sân bay C").STT); 
        //}

        // method trên có hiển thị hộp thoại

        [TestMethod]
        [DataRow("TP. Hồ Chí Minh (SGN), Vietnam", "SGN")]
        [DataRow("Hà Nội (HAN), Vietnam", "HAN")]
        [DataRow("Da Nang (DAD), Vietnam", "DAD")] 
        [DataRow("Invalid String", "Invalid String")] 
        [DataRow("SGN", "SGN")]
        [DataRow(null, "")]
        public void ExtractMaSB(string displayString, string expected)
        {
            var methodInfo = typeof(FlightManagementViewModel).GetMethod("ExtractMaSB", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = (string)methodInfo.Invoke(_viewModel, new object[] { displayString });

            Assert.AreEqual(expected, result);
        }
    }
}