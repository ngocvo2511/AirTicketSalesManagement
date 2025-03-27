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
        private string _canCuocCongDan;
        public string CanCuocCongDan
        {
            get { return _canCuocCongDan; }
            set { _canCuocCongDan = value; }
        }
        private string _maNguoiGiamHo;
        public string MaNguoiGiamHo
        {
            get { return _maNguoiGiamHo; }
            set { _maNguoiGiamHo = value; }
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
    }
}
