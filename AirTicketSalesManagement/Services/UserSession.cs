using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Services
{
    public class UserSession
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        // các thuộc tính khác

        public static UserSession Current { get; } = new UserSession();
    }
}
