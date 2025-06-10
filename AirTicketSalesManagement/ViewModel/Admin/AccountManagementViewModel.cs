using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AirTicketSalesManagement.ViewModel.Admin
{
    public partial class AccountManagementViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ObservableCollection<AdminAccount> adminAccounts = new();

        [ObservableProperty]
        private AdminAccount selectedAccount;

        [ObservableProperty]
        private string newUsername;

        [ObservableProperty]
        private string newEmail;

        public AccountManagementViewModel()
        {
            AdminAccounts.Add(new AdminAccount { Username = "admin1", Email = "admin1@example.com", Role = "Admin" });
            AdminAccounts.Add(new AdminAccount { Username = "admin2", Email = "admin2@example.com", Role = "SuperAdmin" });
        }

        [RelayCommand]
        private void Add(string password)
        {
            if (string.IsNullOrWhiteSpace(NewUsername) || string.IsNullOrWhiteSpace(password)) return;

            AdminAccounts.Add(new AdminAccount
            {
                Username = NewUsername,
                Email = NewEmail,
                Role = "Admin",
                Password = password
            });

            NewUsername = string.Empty;
            NewEmail = string.Empty;
        }

        [RelayCommand]
        private void Edit()
        {
            if (SelectedAccount == null) return;

            SelectedAccount.Username = NewUsername;
            SelectedAccount.Email = NewEmail;
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedAccount != null)
                AdminAccounts.Remove(SelectedAccount);
        }
    }

    public partial class AdminAccount : ObservableObject
    {
        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string role;

        [ObservableProperty]
        private string password;
    }
}
