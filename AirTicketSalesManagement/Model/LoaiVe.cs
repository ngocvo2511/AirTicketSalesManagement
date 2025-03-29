using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Model
{
    public class LoaiVe
    {
        private string _maLoaiVe;
        public string MaLoaiVe
        {
            get { return _maLoaiVe; }
            set { _maLoaiVe = value; }
        }
        private string _maLichBay;
        public string MaLichBay
        {
            get { return _maLichBay; }
            set { _maLichBay = value; }
        }
        private string _hangGhe;
        public string HangGhe
        {
            get { return _hangGhe; }
            set { _hangGhe = value; }
        }
        private int _giaVe;
        public int GiaVe
        {
            get { return _giaVe; }
            set { _giaVe = value; }
        }
        private int _soLuongVeToiDa;
        public int SoLuongVeToiDa
        {
            get { return _soLuongVeToiDa; }
            set { _soLuongVeToiDa = value; }
        }
    }
}
