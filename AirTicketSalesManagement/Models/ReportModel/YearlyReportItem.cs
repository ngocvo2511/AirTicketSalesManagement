using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Models.ReportModel
{
    public class YearlyReportItem
    {
        public int Year { get; set; }
        public string MonthName { get; set; } = "";
        public decimal Revenue { get; set; }
        public int TotalFlights { get; set; }
        public decimal RevenueRate { get; set; }
    }
}
