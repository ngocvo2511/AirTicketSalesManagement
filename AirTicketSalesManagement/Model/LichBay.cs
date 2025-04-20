using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Model
{
    public class LichBay
    {
        private string _maLichBay;
        public string MaLichBay
        {
            get { return _maLichBay; }
            set { _maLichBay = value; }
        }
        private string _soHieuCB;
        public string SoHieuCB
        {
            get { return _soHieuCB; }
            set { _soHieuCB = value; }
        }
        private DateTime _gioDi;
        public DateTime GioDi
        {
            get { return _gioDi; }
            set { _gioDi = value; }
        }
        private DateTime _gioDen;
        public DateTime GioDen
        {
            get { return _gioDen; }
            set { _gioDen = value; }
        }
        private string _loaiMayBay;
        public string LoaiMayBay
        {
            get { return _loaiMayBay; }
            set { _loaiMayBay = value; }
        }
        private string _soLuongVeKhaiThac;
        public string SoluongVekhaiThac
        {
            get { return _soLuongVeKhaiThac; }
            set { _soLuongVeKhaiThac = value; }
        }
        private int _giaVe;
        public int GiaVe
        {
            get{ return _giaVe; }
            set { _giaVe = value; }
        }
        private string _tinhTrangLichBay;
        public string TinhTrangLichBay
        {
            get { return _tinhTrangLichBay; }
            set { _tinhTrangLichBay = value; }
        }

        public ChuyenBay ChuyenBay { get; set; }

    }
}
