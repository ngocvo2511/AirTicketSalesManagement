using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Models.UIModels
{
    public partial class QuanLiDatVe : ObservableObject
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
        public string? HoTenNguoiDat { get; set; }
        public string? EmailNguoiDat { get; set; }
        public int? QdDatVe { get; set; }
        public int? QdHuyVe { get; set; }
        public DateTime? NgayDat { get; set; }
        private string? _trangThai;
        public string? TrangThai
        {
            get => _trangThai;
            set
            {
                if (SetProperty(ref _trangThai, value))
                {
                    OnPropertyChanged(nameof(CanCancel));
                    OnPropertyChanged(nameof(CanConfirm));
                }
            }
        }

        public bool CanCancel
        {
            get
            {
                if (TrangThai == "Đã hủy" || TrangThai == "Chưa thanh toán (Online)" || TrangThai == "Giữ chỗ") return false;
                if (GioDi == null || NgayDat == null || QdHuyVe == null) return false;                
                DateTime thoiDiemToiDaHuy = GioDi.Value.AddDays(-QdHuyVe.Value);
                return DateTime.Now <= thoiDiemToiDaHuy;
            }
        }
        public bool CanConfirm
        {
            get
            {
                if (TrangThai == "Chưa thanh toán (Tiền mặt)")
                {
                    DateTime thoiDiemToiDaXacNhan = GioDi.Value.AddDays(-QdDatVe.Value);
                    return DateTime.Now <= thoiDiemToiDaXacNhan;
                }              
                if (GioDi == null || NgayDat == null || QdDatVe == null) return false;
                return false;
            }
        }
    }
}
