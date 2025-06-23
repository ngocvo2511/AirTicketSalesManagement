using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Models
{
    public partial class HangVe : ObservableObject
    {
        [ObservableProperty]
        private int maHangVe; //ma hangve lich bay

        [ObservableProperty]
        private string tenHangVe;

        [ObservableProperty]
        private decimal giaVe;

        [ObservableProperty]
        private int soGheConLai;

        [ObservableProperty]
        private string backgroundColor;

        [ObservableProperty]
        private string headerColor;

        [ObservableProperty]
        private string buttonColor;
    }
}
