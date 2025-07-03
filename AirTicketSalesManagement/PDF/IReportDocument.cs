using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.PDF
{
    interface IReportDocument
    {
        void Generate(string filePath);
    }
}
