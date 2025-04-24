using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Models
{
    public class ThongTinChuyenBayDuocChon
    {
        public KQTraCuuChuyenBayMoRong Flight { get; set; }
        public HangVe TicketClass { get; set; }
        public int NumberAdults { get; set; }
        public int NumberChildren { get; set; }
        public int NumberInfants { get; set; }
    }
}
