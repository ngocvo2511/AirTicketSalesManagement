using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Models
{
    public class KQLichSuDatVe : ObservableObject
    {
        public int? MaVe { get; set; }
        public string? DiemDi { get; set; }
        public string? DiemDen { get; set; }
        public string? MaDiemDi { get; set; }
        public string? MaDiemDen { get; set; }
        public string? HangHangKhong { get; set; }
        public DateTime? GioDi { get; set; }
        public DateTime? GioDen { get; set; }
        public string? LoaiMayBay { get; set; }
        public int? SoLuongKhach { get; set; }
        public int? QdHuyVe { get; set; }
        public DateTime? NgayDat { get; set; }

        public decimal TongTienTT { get; set; }
        private string? _trangThai;
        public string? TrangThai
        {
            get => _trangThai;
            set
            {
                if (SetProperty(ref _trangThai, value))
                {
                    OnPropertyChanged(nameof(CanCancel));
                    OnPropertyChanged(nameof(CanProcess));
                }
            }
        }

        public bool CanProcess
        {
            get
            {
                return TrangThai == "Chưa thanh toán (Online)";
            }
        }

        public bool CanCancel
        {
            get
            {
                if (TrangThai == "Đã hủy") return false;
                if (GioDi == null || NgayDat == null || QdHuyVe == null) return false;
                if (TrangThai == "Chưa thanh toán (Online)" || TrangThai == "Giữ chỗ")
                {
                    return false;
                }
                DateTime thoiDiemToiDaHuy = GioDi.Value.AddDays(-QdHuyVe.Value);
                return DateTime.Now <= thoiDiemToiDaHuy;
            }
        }
    }
}
