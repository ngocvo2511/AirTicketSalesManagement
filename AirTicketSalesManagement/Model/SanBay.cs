using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Model
{
    public class SanBay
    {
        private string _maSanBay;
        public string MaSanBay
        {
            get { return _maSanBay; }
            set { _maSanBay = value; }
        }
        private string _tenSanBay;
        public string TenSanBay
        {
            get { return _tenSanBay; }
            set { _tenSanBay = value; }
        }
        private string _thanhPho;
        public string ThanhPho
        {
            get { return _thanhPho; }
            set { _thanhPho = value; }
        }
    }
}
