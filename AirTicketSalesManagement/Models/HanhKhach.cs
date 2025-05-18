using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Models
{
    public class HanhKhach
    {
        public string HoTen { get; set; }
        public string CCCD { get; set; }
        public DateTime NgaySinh { get; set; }
        public string GioiTinh { get; set; }

        public string HoTenNguoiGiamHo { get; set; }

        public HanhKhach(string hoTen, DateTime ngaySinh, string gioiTinh, string cccd, string hoTenNguoiGiamHo)
        {
            HoTen = hoTen;
            CCCD = cccd;
            NgaySinh = ngaySinh;
            GioiTinh = gioiTinh;
            HoTenNguoiGiamHo = hoTenNguoiGiamHo;
        }
    }
}
