using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirTicketSalesManagement.ViewModel.Admin;
using AirTicketSalesManagement.Models.UIModels;
using System.Collections.ObjectModel;

namespace AirTicketSalesManagementTests.ViewModel.Admin
{
    [TestClass]
    public class AccountManagementViewModelTests
    {
        private AccountManagementViewModel _viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _viewModel = new AccountManagementViewModel();
        }

        [TestMethod]
        public void ClearSearch()
        {
            _viewModel.SearchEmail = "test@hehe.com";
            _viewModel.SearchRole = "admin";
            _viewModel.ClearSearch();

            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.SearchEmail), "SearchEmail should be cleared");
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.SearchRole), "SearchRole should be cleared");
        }

        [TestMethod]
        public void ResetsFields()
        {
            _viewModel.AddEmail = "hehe@test.com";
            _viewModel.AddPassword = "lmao";
            _viewModel.AddAccount();

            Assert.IsTrue(_viewModel.IsAddPopupOpen, "Add popup should be open");
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.AddEmail), "AddEmail should be reset");
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.AddPassword), "AddPassword should be reset");
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.AddFullName), "AddFullName should be reset");
            Assert.IsTrue(string.IsNullOrEmpty(_viewModel.AddRole), "AddRole should be reset");
        }

        [TestMethod]
        public void CancelAdd_ClosesAddPopup()
        {
            _viewModel.IsAddPopupOpen = true;
            _viewModel.CancelAdd();

            Assert.IsFalse(_viewModel.IsAddPopupOpen, "Add popup should be closed");
        }

        [TestMethod]
        public void CloseAdd_ClosesAddPopup()
        {
            _viewModel.IsAddPopupOpen = true;
            _viewModel.CloseAdd();

            Assert.IsFalse(_viewModel.IsAddPopupOpen, "Add popup should be closed");
        }

        [TestMethod]
        public void EditAccount_WithSelectedAccount()
        {
            var mockAccount = new AccountModel
            {
                Id = 1,
                Email = "edit@test.com",
                VaiTro = "Nhân viên",
                HoTen = "Van A"
            };
            _viewModel.SelectedAccount = mockAccount;
            _viewModel.UserList = new ObservableCollection<UserSelectionModel>();
            _viewModel.EditAccount();

            Assert.IsTrue(_viewModel.IsEditPopupOpen, "Edit popup should be open");
            Assert.AreEqual(mockAccount.Email, _viewModel.EditEmail, "EditEmail should be populated");
            Assert.AreEqual(mockAccount.VaiTro, _viewModel.EditRole, "EditRole should be populated");
            Assert.AreEqual(mockAccount.HoTen, _viewModel.EditFullName, "EditFullName should be populated");
        }

        [TestMethod]
        public void EditAccount_WithNoSelectedAccount()
        {
            _viewModel.SelectedAccount = null;
            _viewModel.EditAccount();

            Assert.IsFalse(_viewModel.IsEditPopupOpen, "Edit popup should not open if no account is selected");
        }

        [TestMethod]
        public void CancelEdit()
        {
            _viewModel.IsEditPopupOpen = true;
            _viewModel.CancelEdit();

            Assert.IsFalse(_viewModel.IsEditPopupOpen, "Edit popup should be closed");
        }

        [TestMethod]
        public void CloseEdit()
        {
            _viewModel.IsEditPopupOpen = true;
            _viewModel.CloseEdit();

            Assert.IsFalse(_viewModel.IsEditPopupOpen, "Edit popup should be closed");
        }

        [TestMethod]
        [DataRow("test@example.com", true)]
        [DataRow("test.example.com", false)]
        [DataRow("@domain.com", false)]
        [DataRow("", false)]
        [DataRow(null, false)]
        public void IsValidEmail(string email, bool expected)
        {
            var methodInfo = typeof(AccountManagementViewModel).GetMethod("IsValidEmail", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (bool)methodInfo.Invoke(_viewModel, new object[] { email });

            Assert.AreEqual(expected, result);
        }
    }
}