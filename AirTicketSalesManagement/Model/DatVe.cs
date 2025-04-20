using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Model
{
    public class DatVe
    {
        private string _maDatVe;
        public string MaDatVe
        {
            get { return _maDatVe; }
            set { _maDatVe = value; }
        }
        private string _maLichBay;
        public string MaLichBay
        {
            get { return _maLichBay; }
            set { _maLichBay = value; }
        }
        private string _maKhachHang;
        public string MaKhachHang
        {
            get { return _maKhachHang; }
            set { _maKhachHang = value; }
        }
        private DateTime _thoiGianDV;
        public DateTime ThoiGianDV
        {
            get { return _thoiGianDV; }
            set { _thoiGianDV = value; }
        }
        private string _soLuongVe;
        public string SoLuongVe
        {
            get { return _soLuongVe; }
            set { _soLuongVe = value; }
        }
        private string _tongTienThanhToan;
        public string TongTienThanhToan
        {
            get { return _tongTienThanhToan; }
            set { _tongTienThanhToan = value; }
        }
        private string _tinhTrangDatVe;
        public string TinhTrangDatVe
        {
            get { return _tinhTrangDatVe; }
            set { _tinhTrangDatVe = value; }
        }

        public LichBay LichBay { get; set; }
        public KhachHang KhachHang { get; set; }
    }
}
