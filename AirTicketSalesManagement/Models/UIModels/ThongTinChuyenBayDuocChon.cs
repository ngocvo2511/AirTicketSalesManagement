using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Models
{
    public class ThongTinChuyenBayDuocChon
    {
        public int Id { get; set; }
        public KQTraCuuChuyenBayMoRong Flight { get; set; }
        public HangVe TicketClass { get; set; }
    }
}
