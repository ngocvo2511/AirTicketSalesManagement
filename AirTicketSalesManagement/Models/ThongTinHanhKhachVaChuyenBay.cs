using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Models
{
    public class ThongTinHanhKhachVaChuyenBay
    {
        public ThongTinChuyenBayDuocChon FlightInfo { get; set; }
        public List<HanhKhach> PassengerList { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
    }
}
