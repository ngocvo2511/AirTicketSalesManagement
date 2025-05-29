using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Models
{
    public class KQLichSuDatVe : ObservableObject
    {
        public int? MaVe {  get; set; }
        public string? DiemDi { get; set; }
        public string? DiemDen { get; set; }
        public string? MaDiemDi { get; set;}
        public string? MaDiemDen { get; set; }
        public string? HangHangKhong { get; set; }
        public DateTime? GioDi { get; set; }
        public DateTime? GioDen { get; set; }
        public string? LoaiMayBay { get; set; }
        public int? SoLuongKhach { get; set; }

        public DateTime? NgayDat { get; set; }
        public string? TrangThai { get; set; }
    }
}
