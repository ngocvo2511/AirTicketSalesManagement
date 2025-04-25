using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.ViewModel.Customer
{
    public partial class BookingHistoryDetailViewModel : BaseViewModel
    {
        private string idTicket;
        private readonly CustomerViewModel parent;
        public BookingHistoryDetailViewModel() { }
        public BookingHistoryDetailViewModel(string idTicket, CustomerViewModel parent)
        {
            this.idTicket = idTicket;
            this.parent = parent;
        }
        [RelayCommand]
        private void GoBack()
        {
            parent.CurrentViewModel = new BookingHistoryViewModel(parent.IdCustomer, parent);
        }
    }
}
