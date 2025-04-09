using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Model
{
    public class KhachHang
    {
        private string _maKhachHang;
        public string MaKhachHang
        {
            get { return _maKhachHang; }
            set { _maKhachHang = value; }
        }
        private string _hoTenKhachHang;
        public string HoTenKhachHang
        {
            get { return _hoTenKhachHang; }
            set { _hoTenKhachHang = value; }
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
        private string _soDienThoai;
        public string SoDienThoai
        {
            get { return _soDienThoai; }
            set { _soDienThoai = value; }
        }
        private string _email;
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }
        private string _giayToTuyThan;
        public string GiayToTuyThan
        {
            get { return _giayToTuyThan; }
            set { _giayToTuyThan = value; }
        }
    }
}
