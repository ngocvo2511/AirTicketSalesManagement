using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirTicketSalesManagement.ViewModel.Admin;
using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.Models.UIModels;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace AirTicketSalesManagementTests.ViewModel.Admin
{
    [TestClass]
    public class ScheduleManagementViewModelTests
    {
        private ScheduleManagementViewModel _viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _viewModel = new ScheduleManagementViewModel();
        }

        [TestMethod]
        public void AddSchedule()
        {
            _viewModel.AddSoHieuCB = "VN123";
            _viewModel.AddLoaiMB = "Boeing 787";
            _viewModel.TicketClassForScheduleList = new ObservableCollection<HangVeTheoLichBay> { new HangVeTheoLichBay() };
            _viewModel.AddScheduleCommand.Execute(null);

            Assert.IsTrue(_viewModel.IsAddSchedulePopupOpen);
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.AddSoHieuCB));
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.AddLoaiMB));
            Assert.IsNotNull(_viewModel.TicketClassForScheduleList);
            Assert.AreEqual(0, _viewModel.TicketClassForScheduleList.Count, "TicketClassForScheduleList should be cleared");
        }

        [TestMethod]
        public void CancelAddSchedule()
        {
            _viewModel.IsAddSchedulePopupOpen = true;
            _viewModel.CancelAddScheduleCommand.Execute(null);

            Assert.IsFalse(_viewModel.IsAddSchedulePopupOpen);
        }

        //[TestMethod]
        //public void EditSchedule_WithValidSchedul()
        //{
        //    var mockSchedule = new Lichbay
        //    {
        //        MaLb = 1,
        //        SoHieuCb = "VN101",
        //        TtlichBay = "Chờ cất cánh",
        //        Datves = new Collection<Datve>() 
        //    };
        //    _viewModel.EditSchedule(mockSchedule);

        //    Assert.IsTrue(_viewModel.IsEditSchedulePopupOpen, "Edit popup should open for a valid schedule");
        //    Assert.AreEqual(mockSchedule.MaLb, _viewModel.EditID);
        //}

        //[TestMethod]
        //public void EditSchedule_WithBookedSchedule()
        //{
        //    var mockSchedule = new Lichbay
        //    {
        //        MaLb = 3,
        //        SoHieuCb = "QH301",
        //        TtlichBay = "Chờ cất cánh",
        //        Datves = new Collection<Datve> { new Datve() } 
        //    };
        //    _viewModel.EditSchedule(mockSchedule);

        //    Assert.IsFalse(_viewModel.IsEditSchedulePopupOpen, "Edit popup should not open for a schedule with bookings");
        //}

        //[TestMethod]
        //public void EditSchedule_WithDepartedSchedule()
        //{
        //    var mockSchedule = new Lichbay
        //    {
        //        MaLb = 2,
        //        SoHieuCb = "VN102",
        //        TtlichBay = "Chờ cất cánh", 
        //        Datves = new Collection<Datve>()
        //    };
        //    _viewModel.EditSchedule(mockSchedule);

        //    Assert.IsFalse(_viewModel.IsEditSchedulePopupOpen, "Edit popup should not open for a departed schedule");
        //}

        [TestMethod]
        public void CancelEditSchedule()
        {
            _viewModel.IsEditSchedulePopupOpen = true;
            _viewModel.CancelEditScheduleCommand.Execute(null);

            Assert.IsFalse(_viewModel.IsEditSchedulePopupOpen);
        }

        [TestMethod]
        public void AddTicketClass()
        {
            _viewModel.TicketClassForScheduleList = new ObservableCollection<HangVeTheoLichBay>();
            _viewModel.TicketClassList = new ObservableCollection<string> { "Phổ thông", "Thương gia" };
            _viewModel.AddTicketClassCommand.Execute(null);
            _viewModel.AddTicketClassCommand.Execute(null);

            Assert.AreEqual(2, _viewModel.TicketClassForScheduleList.Count);
            Assert.AreEqual(1, _viewModel.TicketClassForScheduleList[0].STT);
            Assert.AreEqual(2, _viewModel.TicketClassForScheduleList[1].STT);
        }

        //[TestMethod]
        //public void RemoveAddTicketClass()
        //{
        //    _viewModel.TicketClassForScheduleList = new ObservableCollection<HangVeTheoLichBay>
        //    {
        //        new HangVeTheoLichBay { STT = 1, TenHangVe = "Phổ thông" },
        //        new HangVeTheoLichBay { STT = 2, TenHangVe = "Thương gia" },
        //        new HangVeTheoLichBay { STT = 3, TenHangVe = "Hạng nhất" }
        //    };
        //    var itemToRemove = _viewModel.TicketClassForScheduleList[1]; 
        //    _viewModel.RemoveAddTicketClass(itemToRemove);

        //    Assert.AreEqual(2, _viewModel.TicketClassForScheduleList.Count);
        //    Assert.IsNull(_viewModel.TicketClassForScheduleList.FirstOrDefault(tc => tc.TenHangVe == "Thương gia"));
        //    Assert.AreEqual(1, _viewModel.TicketClassForScheduleList.First(tc => tc.TenHangVe == "Phổ thông").STT);
        //    Assert.AreEqual(2, _viewModel.TicketClassForScheduleList.First(tc => tc.TenHangVe == "Hạng nhất").STT, "STT of the last item should be updated to 2");
        //}

        // hàm trên có popup

        [TestMethod]
        [DataRow("TP. Hồ Chí Minh (SGN), Vietnam", "SGN")]
        [DataRow("Hà Nội (HAN), Vietnam", "HAN")]
        [DataRow("Invalid String", "Invalid String")]
        [DataRow(null, "")]
        public void ExtractMaSB(string displayString, string expected)
        {
            var methodInfo = typeof(ScheduleManagementViewModel).GetMethod("ExtractMaSB", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = (string)methodInfo.Invoke(_viewModel, new object[] { displayString });

            Assert.AreEqual(expected, result);
        }
    }
}