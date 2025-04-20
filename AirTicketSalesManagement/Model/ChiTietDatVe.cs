using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Model
{
    public class ChiTietDatVe
    {
        private string _maChiTietDatVe;
        public string MaChiTietDatVe
        {
            get { return _maChiTietDatVe; }
            set { _maChiTietDatVe = value; }
        }
        private string _maDatVe;
        public string MaDatVe
        {
            get { return _maDatVe; }
            set { _maDatVe = value; }
        }
        private string _hoTenHanhKhach;
        public string HoTenHanhKhach
        {
            get { return _hoTenHanhKhach; }
            set { _hoTenHanhKhach = value; }
        }
        private string _gioiTinh;
        public string GioiTinh
        {
            get { return _gioiTinh; }
            set { _gioiTinh = value; }
        }
        private DateTime _ngaySinh;
        public DateTime NgaySinh
        {
            get { return _ngaySinh; }
            set { _ngaySinh = value; }
        }
        private string _giayToTuyThan;
        public string GiayToTuyThan
        {
            get { return _giayToTuyThan; }
            set { _giayToTuyThan = value; }
        }
        private string _soDienThoaiLienLac;
        public string SoDienThoaiLienLac
        {
            get { return _soDienThoaiLienLac; }
            set { _soDienThoaiLienLac = value; }
        }
        private string _hoTenNguoiGiamHo;
        public string HoTenNguoiGiamHo
        {
            get { return _hoTenNguoiGiamHo; }
            set { _hoTenNguoiGiamHo = value; }
        }
        private string _CCCDNguoiGiamHo;
        public string CCCDNguoiGiamHo
        {
            get { return _CCCDNguoiGiamHo; }
            set { _CCCDNguoiGiamHo = value; }
        }
        private string _maLoaiVe;
        public string MaLoaiVe
        {
            get { return _maLoaiVe; }
            set { _maLoaiVe = value; }
        }
        private string _giaVeThucTe;
        public string GiaVeThucTe
        {
            get { return _giaVeThucTe; }
            set { _giaVeThucTe = value; }
        }

        public DatVe DatVe { get; set; }
        public LoaiVe LoaiVe { get; set; }
    }
}
