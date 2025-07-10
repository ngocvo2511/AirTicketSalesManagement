using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirTicketSalesManagement.ViewModel.Admin;
using AirTicketSalesManagement.ViewModel.Staff; 
using AirTicketSalesManagement.ViewModel.Booking;
using AirTicketSalesManagement.ViewModel.CustomerManagement;

namespace AirTicketSalesManagementTests.ViewModel.Admin
{
    [STATestClass]
    public class AdminViewModelTests
    {
        private AdminViewModel _adminViewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _adminViewModel = new AdminViewModel();
        }

        [TestMethod]
        public void NavigateToAdminProfileCommand_SetsCurrentViewModelToAdminProfileViewModel()
        {
            _adminViewModel.NavigateToAdminProfileCommand.Execute(null);

            Assert.IsInstanceOfType(_adminViewModel.CurrentViewModel, typeof(AdminProfileViewModel));
        }

        //[TestMethod]
        //public void NavigateToHomePageCommand_SetsCurrentViewModelToHomePageViewModel()
        //{
        //    _adminViewModel.NavigateToAdminProfileCommand.Execute(null);
        //    _adminViewModel.NavigateToHomePageCommand.Execute(null);

        //    Assert.IsInstanceOfType(_adminViewModel.CurrentViewModel, typeof(AirTicketSalesManagement.ViewModel.Admin.HomePageViewModel));
        //}

        [TestMethod]
        public void NavigateToTicketManagementCommand_SetsCurrentViewModelToTicketManagementViewModel()
        {
            _adminViewModel.NavigateToTicketManagementCommand.Execute(null);

            Assert.IsInstanceOfType(_adminViewModel.CurrentViewModel, typeof(TicketManagementViewModel));
        }

        [TestMethod]
        public void NavigateToCustomerManagementCommand_SetsCurrentViewModelToCustomerManagementViewModel()
        {
            _adminViewModel.NavigateToCustomerManagementCommand.Execute(null);

            Assert.IsInstanceOfType(_adminViewModel.CurrentViewModel, typeof(CustomerManagementViewModel));
        }

        [TestMethod]
        public void NavigateToReportCommand_SetsCurrentViewModelToReportViewModel()
        {
            _adminViewModel.NavigateToReportCommand.Execute(null);

            Assert.IsInstanceOfType(_adminViewModel.CurrentViewModel, typeof(ReportViewModel));
        }

        [TestMethod]
        public void NavigateToFlightManagementCommand_SetsCurrentViewModelToFlightManagementViewModel()
        {
            _adminViewModel.NavigateToFlightManagementCommand.Execute(null);

            Assert.IsInstanceOfType(_adminViewModel.CurrentViewModel, typeof(FlightManagementViewModel));
        }

        [TestMethod]
        public void NavigateToScheduleManagementCommand_SetsCurrentViewModelToScheduleManagementViewModel()
        {
            _adminViewModel.NavigateToScheduleManagementCommand.Execute(null);

            Assert.IsInstanceOfType(_adminViewModel.CurrentViewModel, typeof(ScheduleManagementViewModel));
        }

        [TestMethod]
        public void NavigateToAccountManagementCommand_SetsCurrentViewModelToAccountManagementViewModel()
        {
            _adminViewModel.NavigateToAccountManagementCommand.Execute(null);

            Assert.IsInstanceOfType(_adminViewModel.CurrentViewModel, typeof(AccountManagementViewModel));
        }

        [TestMethod]
        public void NavigateToRegulationManagementCommand_SetsCurrentViewModelToRegulationManagementViewModel()
        {
            _adminViewModel.NavigateToRegulationManagementCommand.Execute(null);

            Assert.IsInstanceOfType(_adminViewModel.CurrentViewModel, typeof(RegulationManagementViewModel));
        }

        // Lệnh logout() phức tạp hơn vì nó tương tác trực tiếp với Application.Current.MainWindow
        // Không vốn không tồn tại trong môi trường test unit :D
    }
}