using System.ComponentModel;
using System.Windows.Input;

namespace AirTicketSalesManagement.ViewModel.Staff
{
    public class StaffCustomerInfoFillVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        // Implement INotifyPropertyChanged...

        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }

        public ICommand SaveCustomerCommand { get; }

        public StaffCustomerInfoFillVM()
        {
            SaveCustomerCommand = new RelayCommand(_ => Save());
        }

        private void Save()
        {
            // TODO: logic lưu khách hàng mới
        }
    }
}
