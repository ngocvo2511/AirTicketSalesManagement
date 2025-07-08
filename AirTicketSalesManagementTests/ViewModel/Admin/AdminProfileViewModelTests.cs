using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirTicketSalesManagement.ViewModel.Admin;
using System;
using System.Reflection;

namespace AirTicketSalesManagementTests.ViewModel.Admin
{
    [TestClass]
    public class AdminProfileViewModelTests
    {
        private AdminProfileViewModel _viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _viewModel = new AdminProfileViewModel();
        }

        [TestMethod]
        public void OpenEditProfiles()
        {
            _viewModel.HoTen = "Bruh";
            _viewModel.Email = "admin@test.com";
            _viewModel.SoDienThoai = "0123456789";
            _viewModel.CanCuoc = "123456789012";
            _viewModel.GioiTinh = "Nam";
            _viewModel.NgaySinh = new DateTime(1990, 1, 1);
            _viewModel.OpenEditProfileCommand.Execute(null);

            Assert.IsTrue(_viewModel.IsEditPopupOpen, "Edit profile popup should be open");
            Assert.AreEqual(_viewModel.HoTen, _viewModel.EditHoTen);
            Assert.AreEqual(_viewModel.Email, _viewModel.EditEmail);
            Assert.AreEqual(_viewModel.SoDienThoai, _viewModel.EditSoDienThoai);
            Assert.AreEqual(_viewModel.CanCuoc, _viewModel.EditCanCuoc);
            Assert.AreEqual(_viewModel.GioiTinh, _viewModel.EditGioiTinh);
            Assert.AreEqual(_viewModel.NgaySinh, _viewModel.EditNgaySinh);
        }

        [TestMethod]
        public void CloseEditPopup()
        {
            _viewModel.IsEditPopupOpen = true;
            _viewModel.CloseEditPopupCommand.Execute(null);

            Assert.IsFalse(_viewModel.IsEditPopupOpen, "Edit profile popup should be closed");
        }

        [TestMethod]
        public void OpenChangePassword()
        {
            _viewModel.CurrentPassword = "old";
            _viewModel.NewPassword = "new";
            _viewModel.ConfirmPassword = "new";
            _viewModel.HasPasswordError = true;
            _viewModel.PasswordErrorMessage = "Some error";
            _viewModel.OpenChangePasswordCommand.Execute(null);

            Assert.IsTrue(_viewModel.IsChangePasswordPopupOpen, "Change password popup should be open");
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.CurrentPassword));
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.NewPassword));
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.ConfirmPassword));
            Assert.IsFalse(_viewModel.HasPasswordError);
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.PasswordErrorMessage));
        }

        [TestMethod]
        public void CloseChangePasswordPopup()
        {
            _viewModel.IsChangePasswordPopupOpen = true;
            _viewModel.CloseChangePasswordPopupCommand.Execute(null);

            Assert.IsFalse(_viewModel.IsChangePasswordPopupOpen, "Change password popup should be closed");
        }

        [TestMethod]
        [DataRow("test@example.com", true)]
        [DataRow("test.example@domain.co.uk", true)]
        [DataRow("test.example.com", false)]
        [DataRow("test@.com", false)]
        [DataRow("@example.com", false)]
        [DataRow("", false)]
        [DataRow(null, false)]
        public void IsValidEmail(string email, bool expected)
        {
            var methodInfo = typeof(AdminProfileViewModel).GetMethod("IsValidEmail", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = (bool)methodInfo.Invoke(_viewModel, new object[] { email });

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("0912345678", true)] 
        [DataRow("0345678912", true)]
        [DataRow("1234567890", false)]
        [DataRow("012345678", false)] 
        [DataRow("01234567890", false)]
        [DataRow("01234567", false)]
        [DataRow("012345678901", false)]
        [DataRow("09a1234567", false)] 
        [DataRow("", false)] 
        public void IsValidPhone(string phone, bool expected)
        {
            var methodInfo = typeof(AdminProfileViewModel).GetMethod("IsValidPhone", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = (bool)methodInfo.Invoke(_viewModel, new object[] { phone });

            Assert.AreEqual(expected, result);
        }
    }
}