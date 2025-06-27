using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirTicketSalesManagement.ViewModel.Login;

namespace AirTicketSalesManagementTests.ViewModel.Login
{
    [TestClass]
    public class LoginViewModelTests
    {
        private LoginViewModel _viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _viewModel = new LoginViewModel();
        }

        [TestMethod]
        public void Constructor()
        {
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.Email));
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.Password));
            Assert.IsFalse(_viewModel.IsPasswordVisible);
        }

        [TestMethod]
        [DataRow("test@example.com", "password", false, "Valid data should not produce errors")]
        [DataRow("", "password", true, "Empty email should produce an error")]
        [DataRow("test@example.com", "", true, "Empty password should produce an error")]
        [DataRow("invalid-email", "password", true, "Invalid email format should produce an error")]
        [DataRow(null, "password", true, "Null email should produce an error")]
        public void Validate(string email, string password, bool expectedHasErrors, string message)
        {
            _viewModel.Email = email;
            _viewModel.Password = password;
            _viewModel.Validate();

            Assert.AreEqual(expectedHasErrors, _viewModel.HasErrors, message);
        }

        [TestMethod]
        public void Validate_WithInvalidEmail()
        {
            _viewModel.Email = "not-an-email";
            _viewModel.Password = "some_password";
            _viewModel.Validate();
            var errors = _viewModel.GetErrors(nameof(_viewModel.Email));

            Assert.IsNotNull(errors, "Errors collection should not be null for Email property");
            Assert.IsTrue(errors.Cast<string>().Any(e => e.Contains("Tài khoản hoặc mật khẩu không hợp lệ")));
        }

        [TestMethod]
        public void Validate_WithValidData()
        {
            _viewModel.Email = "";
            _viewModel.Password = "";
            _viewModel.Validate();
            Assert.IsTrue(_viewModel.HasErrors, "Should have errors initially");

            _viewModel.Email = "valid@email.com";
            _viewModel.Password = "valid_password";
            _viewModel.Validate();
            Assert.IsFalse(_viewModel.HasErrors, "Errors should be cleared after validation with valid data");
        }
    }
}