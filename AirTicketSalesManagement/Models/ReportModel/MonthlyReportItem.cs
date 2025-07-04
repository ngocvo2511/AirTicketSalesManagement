using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Models.ReportModel
{
    public class MonthlyReportItem
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string FlightNumber { get; set; } = "";
        public string Airline { get; set; } = "";
        public DateTime DepartureTime { get; set; }
        public int TicketsSold { get; set; }
        public decimal Revenue { get; set; }
        public decimal RevenueRate { get; set; }
    }
}
