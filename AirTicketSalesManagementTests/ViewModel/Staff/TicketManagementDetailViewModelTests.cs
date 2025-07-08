using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirTicketSalesManagement.ViewModel.Staff;
using AirTicketSalesManagement.Models.UIModels;

namespace AirTicketSalesManagementTests.ViewModel.Staff
{
    [STATestClass]
    public class TicketManagementDetailViewModelTests
    {
        private StaffViewModel _mockParentViewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockParentViewModel = new StaffViewModel();
        }

        [TestMethod]
        public void GoBack()
        {
            var mockTicket = new QuanLiDatVe();
            var viewModel = new TicketManagementDetailViewModel(mockTicket, _mockParentViewModel);
            var initialViewModel = _mockParentViewModel.CurrentViewModel;
            viewModel.GoBackCommand.Execute(null);

            Assert.IsInstanceOfType(_mockParentViewModel.CurrentViewModel, typeof(TicketManagementViewModel));
            Assert.AreNotSame(initialViewModel, _mockParentViewModel.CurrentViewModel);
        }
    }
}