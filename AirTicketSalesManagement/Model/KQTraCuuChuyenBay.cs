using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Model
{
    public partial class KQTraCuuChuyenBay : ObservableObject
    {
        [ObservableProperty]
        private string maSBDi;
        [ObservableProperty]
        private string maSBDen;
        [ObservableProperty]
        private string hangHangKhong;
        [ObservableProperty]
        private TimeSpan gioDi;
        [ObservableProperty]
        private TimeSpan gioDen;
        [ObservableProperty]
        private TimeSpan thoiGianBay;
        [ObservableProperty]
        private string mayBay;
        [ObservableProperty]
        private decimal giaVe;
    }
}
