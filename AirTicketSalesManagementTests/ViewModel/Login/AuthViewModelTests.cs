using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirTicketSalesManagement.ViewModel.Login;

namespace AirTicketSalesManagementTests.ViewModel.Login
{
    [TestClass]
    public class AuthViewModelTests
    {
        private AuthViewModel _viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _viewModel = new AuthViewModel();
        }

        [TestMethod]
        public void Constructor()
        {
            Assert.IsNotNull(_viewModel.CurrentViewModel);
            Assert.IsInstanceOfType(_viewModel.CurrentViewModel, typeof(LoginViewModel), "Default view model should be LoginViewModel");
        }

        [TestMethod]
        public void NavigateToRegister()
        {
            _viewModel.NavigateToRegister();

            Assert.IsInstanceOfType(_viewModel.CurrentViewModel, typeof(RegisterViewModel));
        }

        [TestMethod]
        public void NavigateToLogin()
        {
            _viewModel.NavigateToRegister();
            _viewModel.NavigateToLogin();

            Assert.IsInstanceOfType(_viewModel.CurrentViewModel, typeof(LoginViewModel));
        }

        [TestMethod]
        public void NavigateToForgotPassword()
        {
            _viewModel.NavigateToForgotPassword();

            Assert.IsInstanceOfType(_viewModel.CurrentViewModel, typeof(ForgotPasswordViewModel));
        }
    }
}