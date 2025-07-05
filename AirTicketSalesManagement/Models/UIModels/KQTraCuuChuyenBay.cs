using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Models
{
    public partial class KQTraCuuChuyenBay : ObservableObject
    {
        [ObservableProperty]
        private string maSBDi;

        [ObservableProperty]
        private string maSBDen;

        [ObservableProperty]
        private string diemDi;

        [ObservableProperty]
        private string diemDen;

        [ObservableProperty]
        private int maLichBay;

        [ObservableProperty]
        private string hangHangKhong;

        [ObservableProperty]
        private DateTime ngayDi;

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

        [ObservableProperty]
        private int soSanBayTrungGian; // 0 = bay thẳng, 1-2 = số điểm dừng
    }
}
