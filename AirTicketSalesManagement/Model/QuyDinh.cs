using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Model
{
    public class QuyDinh
    {
        public int ID { get; set; }
        public int SoSanBay { get; set; }
        public int ThoiGianBayToiThieu { get; set; }
        public int SoSanBayTGToiDa { get; set; }
        public int TGDungMin { get; set; }
        public int TGDungMax { get; set; }
        public int SoHangVe { get; set; }
        public int TGDatVeChamNhat { get; set; }
        public int TGHuyDatVe { get; set; }
    }
}
