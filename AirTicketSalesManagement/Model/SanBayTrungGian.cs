using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Model
{
    public class SanBayTrungGian
    {
        private int _sTT;
        public int STT
        {
            get { return _sTT; }
            set { _sTT = value; }
        }
        private string _maSBTG;
        public string MaSBTG
        {
            get { return _maSBTG; }
            set { _maSBTG = value; }
        }
        private string _soHieuCB;
        public string SoHieuCB
        {
            get { return _soHieuCB; }
            set { _soHieuCB = value; }
        }
        private int _thoiGianDung;
        public int ThoiGianDung
        {
            get { return _thoiGianDung; }
            set { _thoiGianDung = value; }
        }
        private string _ghiChu;
        public string GhiChu
        {
            get { return _ghiChu; }
            set { _ghiChu = value; }
        }
        public SanBay SanBay { get; set; }

        public ChuyenBay ChuyenBay { get; set; }
    }
}
