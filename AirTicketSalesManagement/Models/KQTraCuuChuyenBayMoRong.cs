using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Models
{
    public partial class KQTraCuuChuyenBayMoRong : KQTraCuuChuyenBay
    {
        [ObservableProperty]
        private string logoUrl;

        [ObservableProperty]
        private bool isTicketClassesExpanded;

        [ObservableProperty]
        private ObservableCollection<HangVe> ticketClasses = new();

        public int NumberAdults { get; set; }
        public int NumberChildren { get; set; }
        public int NumberInfants { get; set; }
    }
}
