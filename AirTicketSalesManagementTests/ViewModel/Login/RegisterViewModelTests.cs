using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirTicketSalesManagement.ViewModel.Login;
using System.Threading.Tasks;
using System.Linq;

namespace AirTicketSalesManagementTests.ViewModel.Login
{
    [TestClass]
    public class RegisterViewModelTests
    {
        private RegisterViewModel _viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _viewModel = new RegisterViewModel();
        }

        [TestMethod]
        public void Constructor()
        {
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.Name));
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.Email));
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.Password));
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.ConfirmPassword));
        }

        [TestMethod]
        [DataRow("John Doe", "test@example.com", "Password123", "Password123", false, "Valid data should not produce errors")]
        [DataRow("", "test@example.com", "Password123", "Password123", true, "Empty name should produce an error")]
        [DataRow("John Doe", "", "Password123", "Password123", true, "Empty email should produce an error")]
        [DataRow("John Doe", "test@example.com", "", "Password123", true, "Empty password should produce an error")]
        [DataRow("John Doe", "test@example.com", "Password123", "", true, "Empty confirm password should produce an error")]
        [DataRow("John Doe", "invalid-email", "Password123", "Password123", true, "Invalid email format should produce an error")]
        [DataRow("John Doe", "test@example.com", "Password123", "WrongPassword123", true, "Mismatched passwords should produce an error")]
        public async Task Validate(string name, string email, string password, string confirmPassword, bool expectedHasErrors, string message)
        {
            _viewModel.Name = name;
            _viewModel.Email = email;
            _viewModel.Password = password;
            _viewModel.ConfirmPassword = confirmPassword;
            await _viewModel.Validate();

            Assert.AreEqual(expectedHasErrors, _viewModel.HasErrors, message);
        }

        [TestMethod]
        public async Task Validate_WithMismatchedPasswords()
        {
            _viewModel.Name = "John Doe";
            _viewModel.Email = "test@example.com";
            _viewModel.Password = "Password123";
            _viewModel.ConfirmPassword = "DIFFERENT";
            await _viewModel.Validate();
            var errors = _viewModel.GetErrors(nameof(_viewModel.ConfirmPassword));

            Assert.IsNotNull(errors, "Errors collection for ConfirmPassword should not be null");
            Assert.IsTrue(errors.Cast<string>().Any(e => e.Contains("Xác nhận mật khẩu không khớp")));
        }

        [TestMethod]
        public async Task Validate_WithEmptyName()
        {
            _viewModel.Name = "";
            _viewModel.Email = "test@example.com";
            _viewModel.Password = "Password123";
            _viewModel.ConfirmPassword = "Password123";
            await _viewModel.Validate();
            var errors = _viewModel.GetErrors(nameof(_viewModel.Name));

            Assert.IsNotNull(errors);
            Assert.IsTrue(errors.Cast<string>().Any(e => e.Contains("Tên không được để trống")));
        }

        [TestMethod]
        public async Task Validate_WithValidData()
        {
            _viewModel.Name = "";
            _viewModel.Email = "invalid";
            _viewModel.Password = "123";
            _viewModel.ConfirmPassword = "456";
            await _viewModel.Validate();
            Assert.IsTrue(_viewModel.HasErrors, "Should have errors initially");

            _viewModel.Name = "Valid User";
            _viewModel.Email = "valid@user.com";
            _viewModel.Password = "ValidPass123";
            _viewModel.ConfirmPassword = "ValidPass123";
            await _viewModel.Validate();

            Assert.IsFalse(_viewModel.HasErrors, "Errors should be cleared after validation with valid data");
        }
    }
}