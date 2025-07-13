using System;

namespace AirTicketSalesManagement.Models
{
    public class SearchFlightParameters
    {
        public string DiemDi { get; set; }
        public string DiemDen { get; set; }
        public DateTime? NgayDi { get; set; }
        public int SoLuongGhe { get; set; } = 1;
    }
} 